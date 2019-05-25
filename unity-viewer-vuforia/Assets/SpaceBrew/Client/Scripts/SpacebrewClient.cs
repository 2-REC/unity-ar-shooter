using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WebSocketSharp;
using SimpleJSON;
using System;
using System.Linq;

//TODO: remove all "print"
//TODO: remove TODOs...

public class SpacebrewClient : MonoBehaviour {

    const string SERVER_ADDRESS_STRING = "ws://<ADDRESS>:<PORT>";
    const string ADMIN_REQUEST_MESSAGE = "{\"admin\":[{\"admin\":true}]}";


    public enum DataType {
        BOOLEAN = 0,
        STRING = 1,
        RANGE = 2
    };

//TODO: move to separate class
//=> Different values depending on game
    public enum MessageType {
        DEFAULT, //NONE
        MOVE,
        SHOOT,
        DESTROY
    };

    [Serializable]
    public class Publisher {
        public string name;
        public DataType pubType;
        public MessageType msgType;
        //public string defaultValue;
    }

    [Serializable]
    public class Subscriber {
        public string name;
        public DataType subType;
        public MessageType msgType;
    }


    public class Config {
        public string clientName;
        public string description;
        public List<Publisher> publishers;
        public List<Subscriber> subscribers;
        public string remoteAddress;
    }

    public class SpacebrewMessage {
        public string name;
        public string type;
        public string value;
        public string clientName;
    }

    public class SpacebrewEvent {
        public GameObject sbGo;
        public string sbEvent;
    }


    public string clientName;
    public string descriptionText;
    public string serverAddress;
    public string serverPort;
	public bool isAdmin;
    public Publisher[] publishers;
	public Subscriber[] subscribers;


    private WebSocket conn;
    private bool attemptingReconnect;

//	public ArrayList SpacebrewEvents;
//	List<SpacebrewEvent> spacebrewEvents = new List<SpacebrewEvent>();
//	List<SpacebrewMessage> spacebrewMsgs = new List<SpacebrewMessage>();
    private SpacebrewEvents sbEvents;
//TODO: check if ok with 2 lists
// => Avoids collection modification error?
    private List<SpacebrewMessage> spacebrewNewMsgs;
    private List<SpacebrewMessage> spacebrewMsgs;

    private SpacebrewAdmin sbAdmin;


    void Awake() {
        spacebrewNewMsgs = new List<SpacebrewMessage>();
        spacebrewMsgs = new List<SpacebrewMessage>();

        sbEvents = gameObject.GetComponentInParent<SpacebrewEvents>();
        if (sbEvents == null) {
            throw new Exception("SpacebrewClient: SpacebrewEvent not found in parent object!");
        }

        // if no server address is provided in Editor, read it from PlayerPrefs (if exist)
        if (serverAddress == "") {
            print("No server address in Editor, reading from PlayerPrefs (or using default)");

            // find a ServerManager
            IServerManager serverManager = null;
            var listServerManagers = FindObjectsOfType<MonoBehaviour>().OfType<IServerManager>();
            if ((listServerManagers == null) || (listServerManagers.Count() == 0)) {
                throw new Exception("A SERVER MANAGER IMPLEMENTATION MUST EXIST IN THE SCENE IF THE SERVER ADDRESS IS NOT SPECFIED IN THE EDITOR!");
            }
            serverManager = listServerManagers.First();

            serverAddress = serverManager.GetServerAddress();
            serverPort = serverManager.GetServerPort();
        }
        string connectionString = SERVER_ADDRESS_STRING.Replace("<ADDRESS>", serverAddress).Replace("<PORT>", serverPort);
        print("Server address: " + connectionString);

        if (isAdmin) {
            sbAdmin = gameObject.GetComponentInParent<SpacebrewAdmin>();
            if (sbAdmin == null) {
                throw new Exception("SPACEBREWADMIN NOT FOUND IN PARENT OBJECT! - Required if 'Is Admin' is set to 'true'");
            }
            print("Admin client");

            var config = new Config {
                clientName = clientName,
                description = descriptionText,
                publishers = new List<Publisher>(publishers),
                subscribers = new List<Subscriber>(subscribers),
//TODO: CHANGE THIS!? WRONG ADDRESS!?
                remoteAddress = serverAddress
            };
            sbAdmin.SetConfig(config);
        }

        conn = new WebSocket(connectionString);

        conn.OnOpen += (sender, e) => {
            print ("Attempting to open socket");
        };

        if (isAdmin) {
            conn.OnMessage += (sender, e) => {
//TODO: REMOVE THIS PRINT - DEBUG PURPOSE
print ("onMessage: " + e.Data);

                // parse the incoming json message from spacebrew
                var N = JSON.Parse(e.Data);

                JSONNode targetType = N["targetType"];
                JSONNode message = N["message"];
                if ((targetType == null) && (message != null)) {
                    HandleMessage(message);
                }
                else {
                    if (targetType.ToString() == "\"admin\"") {
                        HandleAdminMessage(N);
                    }
                    else {
                        JSONNode route = N[0]["route"];
                        if (route != null) {
//TODO: REMOVE THIS PRINT - DEBUG PURPOSE
print("remove-route message - disconnected client");
                            HandleRouteMessage(route);
                        }
                        else {
                            //TODO: handle other messages?
                            print("Not admin message - unhandled");
                        }
                    }
                }
            };
        }
        else {
            conn.OnMessage += (sender, e) => {
//TODO: REMOVE THIS PRINT - DEBUG PURPOSE
print ("onMessage: " + e.Data);

                // parse the incoming json message from spacebrew
                var N = JSON.Parse(e.Data);

                JSONNode message = N["message"];
                if (message != null) {
                    HandleMessage(message);
                }

//TODO: remove this commented code... (use?)
//  			if (e.Type == Opcode.Text) {
//  				// Do something with e.Data
//  				print (e);
//  				print (e.Data);
//  				return;
//  			}
//
//  			if (e.Type == Opcode.Binary) {
//  				// Do something with e.RawData
//  				return;
//  			}
            };
        }

		conn.OnError += (sender, e) => {
			print ("THERE WAS AN ERROR CONNECTING");
			print (e.Message);
		};

		conn.OnClose += (sender, e) => {
			print ("Connection closed");


        };


        print("Attemping to connect to " + connectionString);
		conn.Connect ();

		//addPublisher ("power", "boolean", "0");
		//addSubscriber ("hits", "boolean");

		// Special message to Spacebrew to act as admin interface
		if (isAdmin) {
			print("Sending admin request");
			conn.Send(ADMIN_REQUEST_MESSAGE);
		}

//TODO: check if still need to send this message when sending the previous admin one
		// Connect and send the configuration for the app to Spacebrew
//		conn.Send (makeConfig().ToString());
		SendConfigMessage();
	}

	// You can use these to programatically add publisher and subsribers
	// otherwise you should do it through the editor interface.
	void addPublisher(string _name, string _type, string _default) {
		var P = new JSONClass();
		P ["name"] = _name;
		P ["type"] = _type;
//		if (_default != "") {
//			P ["default"] = _default;
//		}
//TODO: uncomment?!
		//publishers.Add(P);
	}

	void addSubscriber(string _name, string _type) {
		var S = new JSONClass();
		S ["name"] = _name;
		S ["type"] = _type;
//TODO: uncomment?!
		//subscribers.Add(S);
	}

	private JSONClass makeConfig() {
		// Begin the JSON config
		var I = new JSONClass();
		I["name"] = clientName;
		I["description"] = descriptionText;

		// Add all the publishers
		print ("there are " + publishers.Length);
		for (int i = 0; i < publishers.Length; i++) // Loop through List with for
		{
			var O = new JSONClass();
			O["name"] = publishers[i].name;
			string tType = "empty";
			switch (publishers[i].pubType) {
			case DataType.BOOLEAN:
				tType = "boolean";
				break;
			case DataType.STRING:
				tType = "string";
				break;
			case DataType.RANGE:
				tType = "range";
				break;
			}
			O["type"] = tType;
            O["msgType"] = publishers[i].msgType.ToString();
            O["default"] = "";
			I["publish"] ["messages"][-1] = O;
		}

		// Add all the subscribers
		for (int i = 0; i < subscribers.Length; i++) // Loop through List with for
		{
			var Q = new JSONClass();
			Q["name"] = subscribers[i].name;
			string tType = "empty";
			switch (subscribers[i].subType) {
			case DataType.BOOLEAN:
				tType = "boolean";
				break;
			case DataType.STRING:
				tType = "string";
				break;
			case DataType.RANGE:
				tType = "range";
				break;
			}
			Q["type"] = tType;
            Q["msgType"] = subscribers[i].msgType.ToString();
            I["subscribe"] ["messages"][-1] = Q;
		}


		// Add everything to config
		var C = new JSONClass();
		C ["config"] = I;

		print("Connection:");
		print(C.ToString());
		print("");

		return C;
	}

//TODO: remove? (unused)
	void onOpen() {

	}

//TODO: remove? (unused)
	void onClose() {

	}

/*
	public void addEventListener(GameObject _sbGo, string _event) {
		print ("Adding a listener for " + _event);
		SpacebrewEvent evt = new SpacebrewEvent();
		evt.sbGo = _sbGo;
		evt.sbEvent = _event;
		spacebrewEvents.Add(evt);
	}
*/

//TODO: remove? (unused)
	public void sendMessage(string _name, string _type, string _value) {
		var M = new JSONClass();
		M["clientName"] = clientName;
		M["name"] = _name;
		M["type"] = _type;
		M["value"] = _value;

		var MS = new JSONClass ();
		MS ["message"] = M;
		conn.Send(MS.ToString());
////        conn.Send (makeConfig().ToString());
//        SendConfigMessage();

//       {
//         "message":{
//           "clientName":"CLIENT NAME (Must match the name in the config statement)",
//           "name":"PUBLISHER NAME (outgoing messages), SUBSCRIBER NAME (incoming messages)",
//           "type":"DATA TYPE",
//           "value":"VALUE",
//       }
//   }
	}

    public void SendConfigMessage() {
        JSONClass message = makeConfig();
        conn.Send(message.ToString());
    }

    public void SendRouteMessage(bool add, string publisherClientName, Publisher publisher, string publisherAddress,
            string subscriberClientName, Subscriber subscriber, string subscriberAddress) {

        var publisherNode = new JSONClass {
            ["clientName"] = publisherClientName,
            ["name"] = publisher.name,
            ["type"] = publisher.pubType.ToString().ToLower(),
            ["remoteAddress"] = publisherAddress
        };

        var subscriberNode = new JSONClass {
            ["clientName"] = subscriberClientName,
            ["name"] = subscriber.name,
            ["type"] = subscriber.subType.ToString().ToLower(),
            ["remoteAddress"] = subscriberAddress
        };

		var routeNode = new JSONClass {
            ["type"] = add ? "add" : "remove",
            ["publisher"] = publisherNode,
            ["subscriber"] = subscriberNode,
        };

        var message = new JSONClass {
            ["route"] = routeNode,
            ["targetType"] = "admin"
        };

        conn.Send(message.ToString());
	}

//TODO: useless?
    public void SendRemoveMessage() {

        var removeNode = new JSONClass {
            ["name"] = clientName,
//TODO: CHANGE THIS!? WRONG ADDRESS!?
            ["remoteAddress"] = serverAddress
        };

        var message = new JSONClass();
        message["remove"][-1] = removeNode;
        message["targetType"] = "admin";

print("REMOVE MESSAGE: " + message.ToString());
        conn.Send(message.ToString());
    }

//TODO: remove? (unused)
    void ProcessSpacebrewMessage(SpacebrewMessage _cMsg) {
//		foreach (SpacebrewEvent element in spacebrewEvents)
//		{
//			//This will now work because you've constrained the generic type V
//			print(element.sbEvent);
//			if (_cMsg.name == element.sbEvent) {
//
//				// if this element subscribes to this event then call it's callback
//				//element.eventCallback
//				//element.sbGo.OnSpacebrewEvent(_cMsg);
//				element.sbGo.SendMessage("OnSpacebrewEvent", _cMsg);
//				//this.GetComponent<SpacebrewEvents>().OnSpacebrewEvent(_cMsg);
//				//this.GetComponent<MyScript>().MyFunction();
//				//print(element.sbGo);
//				//element.sbGo.gameObject.SpacebrewEvent(_cMsg);
//				//element.sbGo.
////				MethodInfo mi = element.sbGo.GetType().GetMethod(element.eventCallback);
//				//mi.Invoke(element.sbGo, null);
//			}
//		}
		if (_cMsg.name == "hits") {
			if (_cMsg.value == "true"){
				print ("do something");
					//pillVisible = !pillVisible;
				}
			}
	}

/*
	// Use this for initialization
	void Start () {

	}
*/

/*
	// Update is called once per frame
	void Update () {

		// go through new messages
		foreach (SpacebrewMessage element in spacebrewMsgs)
		{
			//This will now work because you've constrained the generic type V
			//print(element.sbEvent);
			//if (_cMsg.name == element.sbEvent) {

				// if this element subscribes to this event then call it's callback
				//element.eventCallback
				//element.sbGo.OnSpacebrewEvent(_cMsg);
				//element.sbGo.SendMessage("OnSpacebrewEvent", _cMsg);
				this.GetComponent<SpacebrewEvents>().OnSpacebrewEvent(element);
				//this.GetComponent<MyScript>().MyFunction();
				//print(element.sbGo);
				//element.sbGo.gameObject.SpacebrewEvent(_cMsg);
				//element.sbGo.
				//				MethodInfo mi = element.sbGo.GetType().GetMethod(element.eventCallback);
				//mi.Invoke(element.sbGo, null);
			//}
		}
		spacebrewMsgs.Clear();

		if (Input.GetKeyDown ("space")) {
			print ("Sending Spacebrew Message");
			//sendMessage();
		}
		//GameObject.Find("pill").renderer.enabled = pillVisible;

		//check to see if connection has died, connect if so
		if (conn.ReadyState != WebSocketState.OPEN && !attemptingReconnect ){
			StartCoroutine( "AttemptWebsocketReconnect" );
		}
	}
*/
	void Update() {
        var tmpSpacebrewMsgs = spacebrewMsgs;
        spacebrewMsgs = spacebrewNewMsgs;
        spacebrewNewMsgs = tmpSpacebrewMsgs;

        // go through new messages
        //TODO: remove messages as soon as handled (inside loop) - to avoid "modified collection" error
        foreach (SpacebrewMessage element in spacebrewMsgs) {
            sbEvents.OnSpacebrewEvent(element);
		}
		spacebrewMsgs.Clear();

		//check to see if connection has died, connect if so
		if ((conn.ReadyState != WebSocketState.OPEN) && !attemptingReconnect) {
			StartCoroutine("AttemptWebsocketReconnect");
		}
	}

	private IEnumerator AttemptWebsocketReconnect(){
		attemptingReconnect = true;

		float timer = 0.1f;
		float maxInterval = 3.0f;
		while (conn.ReadyState != WebSocketState.OPEN)
        {
            Debug.LogWarning("Attempting to Reconnect");
            conn.ConnectAsync ();
			yield return new WaitForSeconds (timer);
			if (timer < maxInterval) {
				timer *= 2.0f; // exponential backoff
			} else {
				timer = maxInterval;
			}
		}
		// Special message to Spacebrew to act as admin interface
		if (isAdmin) {
			print("Sending admin request");
			conn.Send(ADMIN_REQUEST_MESSAGE);
		}
//TODO: check if still need to send this message when sending the previous admin one
		SendConfigMessage();

		attemptingReconnect = false;
	}

    private void OnApplicationQuit() {

////
//TODO: don't do this here!
// (maybe here anyway, to make sure?)
// => call when leave scene (same as quit?), and add a variable "leaving/stop" to check in Update & avoid trying to reconnect!
//TODO: need to find way to check "live" connections (?)
//TODO: useless? (automatically sent by server when client disconnect)
//        SendRemoveMessage();
//TODO: should wait before disconnect?
//TODO: check that "conn" object exist and is valid! (& still connected)
        conn.Close();

        if (isAdmin) {
            sbAdmin.Clear();
            // - if server => stop server
        }
////

    }


    private void HandleMessage(JSONNode message) {
        var sbMsg = new SpacebrewMessage {
            name = message["name"],
            type = message["type"],
            value = message["value"],
            clientName = message["clientName"]
        };
//TODO: REMOVE THIS PRINT - DEBUG PURPOSE
print(sbMsg);
//        spacebrewMsgs.Add(sbMsg);
        spacebrewNewMsgs.Add(sbMsg);
//TODO: might be better to handle message directly instead of putting in messages queue?
        //ProcessSpacebrewMessage(cMsg);
    }

    private void HandleAdminMessage(JSONNode message) {
        if (message["admin"] != null) {
            print("\"admin\" message - ignored");
            //TODO: what to do?
            //...
        }
        else if (message["config"] != null) {
            print("\"config\" message");
            Config config = getConfig(message["config"]);
            sbAdmin.OnConfig(config);
        }
        else if (message["remove"] != null) {
            print("\"remove\" message");
//TODO: might have more than 1 element in array?
            sbAdmin.OnRemove(message["remove"][0]["name"]);
        }
        else if (message["route"] != null) {
            print("\"route\" message - ignored");
            //TODO: handle? => could be useful (debug purpose)
            //...
        }
        else if (message["message"] != null) {
            print("\"message\" message - ignored");
            //TODO: what to do? ignore?
            //...
        }
        else {
            print("unknown message type");
            //TODO: what to do? ignore?
            //...
        }
    }

    private void HandleRouteMessage(JSONNode message) {
        if ((message["type"].ToString() == "\"remove\"") && (message["client_disconnect"].ToString() == "\"true\"")) {

            JSONNode publisherNode = message["publisher"];
            var publisher = new Publisher {
                name = publisherNode["name"],
                pubType = GetDataType(publisherNode["type"])
            };

            JSONNode subscriberNode = message["subscriber"];
            var subscriber = new Subscriber {
                name = subscriberNode["name"],
                subType = GetDataType(subscriberNode["type"])
            };

            sbAdmin.OnRouteRemove(publisherNode["clientName"], publisher, publisherNode["remoteAddress"],
                    subscriberNode["clientName"], subscriber, subscriberNode["remoteAddress"]);
        }
    }



    private static Config getConfig(JSONNode message) {
        var config = new Config {
            clientName = message["name"],
            description = message["description"],
            publishers = GetPublishers(message["publish"]["messages"]),
            subscribers = GetSubscribers(message["subscribe"]["messages"]),
            remoteAddress = message["remoteAddress"]
        };
        return config;
    }

    private static List<Publisher> GetPublishers(JSONNode publishersNode) {
        List<Publisher> publishers = new List<Publisher>();

        int count = 0;
        JSONNode node = publishersNode[count];
        while (node != null) {
print("pub " + count);
            var publisher = new Publisher {
                name = node["name"],
//                pubType = GetDataType(node["type"].ToString()),
                pubType = GetDataType(node["type"]),
//                msgType = GetMessageType(node["msgType"].ToString()),
                msgType = GetMessageType(node["msgType"])
            };

            publishers.Add(publisher);
            ++count;
            node = publishersNode[count];
        }
print("nb publishers: " + count);
        return publishers;
    }

    private static List<Subscriber> GetSubscribers(JSONNode subscribersNode) {
        List<Subscriber> subscribers = new List<Subscriber>();

        int count = 0;
        JSONNode node = subscribersNode[count];
        while (node != null) {
print("sub " + count);
            var subscriber = new Subscriber {
                name = node["name"],
//                subType = GetDataType(node["type"].ToString()),
                subType = GetDataType(node["type"]),
//                msgType = GetMessageType(node["msgType"].ToString()),
                msgType = GetMessageType(node["msgType"])
            };

            subscribers.Add(subscriber);
            ++count;
            node = subscribersNode[count];
        }
print("nb subscribers: " + count);
        return subscribers;
    }

    private static DataType GetDataType(string type) {
        DataType dataType = DataType.STRING;
print("get data type " + type);
        switch (type) {
            case "boolean":
                dataType = DataType.BOOLEAN;
                break;
            case "string":
print("STRING");
                dataType = DataType.STRING;
                break;
            case "range":
                dataType = DataType.RANGE;
                break;
        }
        return dataType;
    }

    private static MessageType GetMessageType(string type) {
        MessageType msgType = MessageType.DEFAULT;
print("get msg type " + type);
        foreach (MessageType enumType in Enum.GetValues(typeof(MessageType))) {
            if (type == enumType.ToString()) {
                msgType = enumType;
                break;
            }
        }
        return msgType;
    }

}
