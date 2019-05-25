using UnityEngine;
using System.Collections.Generic;
using static SpacebrewClient;
using System;

public class SpacebrewAdmin : MonoBehaviour {

    private class Route {
        public bool fromServer;
        public Publisher publisher;
        public Subscriber subscriber;
    }

    private class ClientRoutes {
        public List<Route> routes = new List<Route>();
        public string address;
    }


    private SpacebrewClient sbClient;
    private Config serverConfig;
    private List<Config> clientConfigs;
    private Dictionary<string, ClientRoutes> routes;


    private void Awake() {
        sbClient = gameObject.GetComponentInParent<SpacebrewClient>();
        if (sbClient == null) {
            throw new Exception("SpacebrewEvent: SpacebrewClient not found in parent object!");
        }

        clientConfigs = new List<Config>();
        routes = new Dictionary<string, ClientRoutes>();
    }

    public void SetConfig(Config config) {
        serverConfig = config;
    }

    public void Clear() {
        foreach (string clientName in routes.Keys) {
            RemoveRoutes(clientName);
        }
        routes.Clear();

        clientConfigs.Clear();
    }

    public void OnConfig(Config config) {
        if ((serverConfig.clientName != config.clientName)
                || (serverConfig.remoteAddress != config.remoteAddress)) {
            clientConfigs.Add(config);
            AddRoutes(config);
        }
//TODO:
// else => get "remoteAddress" from message? (to get IP of machine...)

    }

    public void OnRouteRemove(string publisherClientName, Publisher publisher, string publisherAddress,
                string subscriberClientName, Subscriber subscriber, string subscriberAddress) {
        bool fromServer;
        if (routes.TryGetValue(publisherClientName, out ClientRoutes clientRoutes)) {
            fromServer = false;
        }
        else if (routes.TryGetValue(subscriberClientName, out clientRoutes)) {
            fromServer = true;
        }
        else {
            print("Unknown route: client name not found - ignoring");
            return;
        }

        if ((fromServer ? subscriberAddress : publisherAddress) != clientRoutes.address) {
            print("Unknown route: no correspoding IP address - ignoring");
            return;
        }

        Route route = clientRoutes.routes.Find(x => ((x.fromServer == fromServer) && (x.publisher.name == publisher.name) && (x.subscriber.name == subscriber.name)));
        clientRoutes.routes.Remove(route);
        if (clientRoutes.routes.Count == 0) {
            routes.Remove(fromServer ? subscriberClientName : publisherClientName);
        }
    }

    public void OnRemove(string clientName) {
        RemoveRoutes(clientName);

        Config config = clientConfigs.Find(x => (x.clientName == clientName));
        clientConfigs.Remove(config);

    }


    private void AddRoutes(Config config) {
        // client publishers
        foreach (Publisher publisher in config.publishers) {
            if (publisher.msgType != MessageType.DEFAULT) {
                foreach (Subscriber serverSubscriber in serverConfig.subscribers) {
                    if (serverSubscriber.msgType == publisher.msgType) {
                        AddRoute(false, config.clientName, publisher, config.remoteAddress, serverConfig.clientName, serverSubscriber, serverConfig.remoteAddress);
                    }
                }
            }
        }

        // client subscribers
        foreach (Subscriber subscriber in config.subscribers) {
            if (subscriber.msgType != MessageType.DEFAULT) {
                foreach (Publisher serverPublisher in serverConfig.publishers) {
                    if (serverPublisher.msgType == subscriber.msgType) {
                        AddRoute(true, serverConfig.clientName, serverPublisher, serverConfig.remoteAddress, config.clientName, subscriber, config.remoteAddress);
                    }
                }
            }
        }
    }

    private void AddRoute(bool fromServer, string publisherClientName, Publisher publisher, string publisherAddress,
            string subscriberClientName, Subscriber subscriber, string subscriberAddress) {
//TODO: should instead be called when receive a "route-add" message!
//=> could have a "pending created routes", waiting for server message to save locally
// (then this function would be useless, only containing "sbClient.SendRouteMessage" - similar to "RemoveRoutes")
        SaveRoute(fromServer, publisherClientName, publisher, publisherAddress, subscriberClientName, subscriber, subscriberAddress);

        // send "add route" message to server
        sbClient.SendRouteMessage(true, publisherClientName, publisher, publisherAddress, subscriberClientName, subscriber, subscriberAddress);
    }

    private void RemoveRoutes(string clientName) {
        if (routes.TryGetValue(clientName, out ClientRoutes clientRoutes)) {
            foreach (Route route in clientRoutes.routes) {
                if (route.fromServer) {
                    sbClient.SendRouteMessage(false, serverConfig.clientName, route.publisher, serverConfig.remoteAddress, clientName, route.subscriber, clientRoutes.address);
                }
                else {
                    sbClient.SendRouteMessage(false, clientName, route.publisher, clientRoutes.address, serverConfig.clientName, route.subscriber, serverConfig.remoteAddress);
                }
            }

//TODO: should instead have "DeleteRoute" (to remove a single route, called when receive a "route-remove" message!), and check when empty for "clientName" before removing it.
//=> could have a "pending deleted routes", waiting for server message to delete locally
            routes.Remove(clientName);
        }
    }


//TODO: should be called by "OnRoute" (when receiving "route-remove" message)
    private void SaveRoute(bool fromServer, string publisherClientName, Publisher publisher, string publisherAddress,
            string subscriberClientName, Subscriber subscriber, string subscriberAddress) {
        string clientName = fromServer ? subscriberClientName : publisherClientName;
        string clientAddress = fromServer ? subscriberAddress : publisherAddress;

        if (!routes.TryGetValue(clientName, out ClientRoutes clientRoutes)) {
            clientRoutes = new ClientRoutes();
            clientRoutes.address = clientAddress;
            routes.Add(clientName, clientRoutes);
        }
        else {
            if (clientRoutes.address != clientAddress) {
                print("ERROR: different address for client!");
//TODO: OK? or should throw exception?
                return;
            }
        }

//TODO: check that ok with fields names
        var route = new Route {
            fromServer = fromServer,
            publisher = publisher,
            subscriber = subscriber,
        };
        clientRoutes.routes.Add(route);
    }

}
