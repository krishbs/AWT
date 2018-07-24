using System;

using System.Threading ;

using System.Web;

using System.Collections;

using System.Collections.Specialized;

using System.Text;

namespace ASPNETChat

{

	/// <summary>

	/// The business logic of the chat application

	/// </summary>

	public class ChatEngine

	{

		private static Hashtable Rooms = new Hashtable(40); 

		public ChatEngine()

		{

		}


		/// <summary>

		/// Deletes the empty chat rooms

		/// </summary>

		/// <param name="state"></param>

		public static void CleanChatRooms(object state)

		{

			Monitor.Enter(Rooms);

			foreach(object key in Rooms.Keys)

			{

				ChatRoom room=(ChatRoom)Rooms[key.ToString()];

				room.ExpireUsers(100);

				if (room.IsEmpty())

				{

					room.Dispose();

					Rooms.Remove(key.ToString());

				}

			}

			Monitor.Exit(Rooms);

		}

		/// <summary>

		/// Returns the chat room for this two users or create a new one if nothing exists

		/// </summary>

		/// <param name="user1ID"></param>

		/// <param name="user2ID"></param>

		/// <returns></returns>

		public static ChatRoom GetRoom(string user1ID,string user2ID)

		{

			return GetRoom(user1ID,"",user2ID,"");

		}

		/// <summary>

		/// Returns or creats a chat room for these two users or create a new one if nothing exists

		/// </summary>

		/// <param name="user1ID"></param>

		/// <param name="user1Name"></param>

		/// <param name="user2ID"></param>

		/// <param name="user2Name"></param>

		/// <returns></returns>

		private static ChatRoom GetRoom(string user1ID,string user1Name,string user2ID,string user2Name) 

		{

			ChatRoom room=null;

			string rid1=CreateRoomID(user1ID,user2ID);

			string rid2=CreateRoomID(user2ID,user1ID);

			Monitor.Enter(Rooms);


			if (Rooms.Contains(rid1))

				room=(ChatRoom)Rooms[rid1];

			else

			{

				if (Rooms.Contains(rid2))

					room=(ChatRoom)Rooms[rid2];

				else

				{

					//if (user1Name=="" && user2Name=="")

					// return null;

					//else

					room=new ChatRoom(user1ID,user1Name,user2ID,user2Name);

					Rooms.Add(rid1,room);

				}


			}

			Monitor.Exit(Rooms);

			return room;

		}


		#region Room ID,User IDs Manipulation

		/// <summary>

		/// Creates the room id using the ids of the two users

		/// </summary>

		/// <param name="user1"></param>

		/// <param name="user2"></param>

		/// <returns></returns>

		public static string CreateRoomID(string user1,string user2)

		{


			user1=user1.ToUpper();

			user2=user2.ToUpper();

			return user1+";"+user2;

		}

		#endregion

		#region Delete Room

		/// <summary>

		/// Deletes the specified room

		/// </summary>

		/// <param name="roomID"></param>

		public static void DeleteRoom(string roomID)

		{

			Monitor.Enter(Rooms);

			ChatRoom room=(ChatRoom)Rooms[roomID];

			room.Dispose();

			Rooms.Remove(roomID);

			Monitor.Exit(Rooms);

		}

		public static void DeleteRoom(string user1id,string user2id)

		{

			DeleteRoom(CreateRoomID(user1id,user2id));

		}

		#endregion

	}

 


	public class ChatRoom : IDisposable

	{

		//public Hashtable activeUsers = null;

		public ArrayList messages = null;

		public string RoomID;

		private ChatUser FirstUser;

		private ChatUser SecondUser;

		public void Dispose()

		{


			this.messages.Clear();

			this.RoomID="";

			this.FirstUser.Dispose();

			this.SecondUser.Dispose();

		}

		/// <summary>

		/// Returns the user with the specified id

		/// </summary>

		/// <param name="userID"></param>

		/// <returns></returns>

		public ChatUser GetUser(string userID)

		{

			userID=userID.ToUpper();

			if (FirstUser.UserID.ToUpper()==userID)

				return FirstUser;

			else

				return SecondUser;

		}


		#region constructors

		public ChatRoom(string user1ID,string user1Name,string user2ID,string user2Name) 

		{

			this.messages = new ArrayList();

			this.RoomID=ChatEngine.CreateRoomID(user1ID,user2ID);

			this.FirstUser=new ChatUser(user1ID,user1Name);

			this.SecondUser=new ChatUser(user2ID,user2Name);

		}

		#endregion


		/// <summary>

		/// Determines if the users of the room are active or not

		/// </summary>

		/// <returns></returns>

		public bool IsEmpty()

		{

			lock(this)

			{

				if (this.FirstUser.IsActive==false&& this.SecondUser.IsActive==false)

					return true;

				else

					return false;

			}

		}

 

		#region Operations Join,Send,Leave

		//

		/// <summary>

		/// Marks the user as inactive

		/// </summary>

		/// <param name="userID"></param>

		/// <returns></returns>

		public string LeaveRoom(string userID)

		{

			//deactivate user 

			ChatUser user=this.GetUser(userID);

			user.IsActive=false;

			user.LastSeen=DateTime.Now;


			//Add leaving message

			Message msg = new Message(user.UserName ,"",MsgType.Left);

			this.AddMsg(msg);

			//Get all the messages to the user

			int lastMsgID;

			ArrayList previousMsgs= this.GetMessagesSince( user.LastMessageReceived,out lastMsgID);

			user.LastMessageReceived=lastMsgID;

			//return the messages to the user

			string str=GenerateMessagesString(previousMsgs);

			if (IsEmpty())

				ChatEngine.DeleteRoom(this.RoomID);

			return "";

		}

		/// <summary>

		/// Activates the user and adds a join message to the room

		/// </summary>

		/// <param name="userID"></param>

		/// <param name="userName"></param>

		/// <returns>All the messages sent in the room</returns>

		public string JoinRoom(string userID,string userName)

		{

			//activate user 

			ChatUser user=this.GetUser(userID);

			user.IsActive=true;

			user.UserName=userName;

			user.LastSeen=DateTime.Now;

			//Add join message

			Message msg=new Message(user.UserName ,"",MsgType.Join);

			this.AddMsg(msg);


			//Get all the messages to the user

			int lastMsgID;

			ArrayList previousMessages=this.GetMessagesSince(-1,out lastMsgID);

			user.LastMessageReceived=lastMsgID;


			//return the messages to the user

			string str=GenerateMessagesString(previousMessages);

			return str;

		}

		/// <summary>

		/// Adds a message in the room

		/// </summary>

		/// <param name="strMsg"></param>

		/// <param name="senderID"></param>

		/// <param name="toUserID"></param>

		/// <returns>All the messages sent from the other user from the last time the user sent a message</returns>

		public string SendMessage(string strMsg,string senderID,string toUserID)

		{

			ChatUser user=this.GetUser(senderID);

			Message msg=new Message(user.UserName ,strMsg,MsgType.Msg);

			user.LastSeen=DateTime.Now;

			this.ExpireUsers(100);

			this.AddMsg(msg);

			int lastMsgID;

			ArrayList previousMsgs= this.GetMessagesSince( user.LastMessageReceived,out lastMsgID);

			if (lastMsgID!=-1)

				user.LastMessageReceived=lastMsgID;

			string res=this.GenerateMessagesString(previousMsgs);

			return res;

		}

		#endregion 

		/// <summary>

		/// Removes the users that hasn't sent any message during the last window secondes 

		/// </summary>

		/// <param name="window">time in secondes</param>

		public void ExpireUsers(int window) 

		{

			lock(this) 

			{

				if (this.FirstUser.LastSeen != System.DateTime.MinValue )

				{

					TimeSpan span = DateTime.Now - this.FirstUser.LastSeen;

					if (span.TotalSeconds > window && this.FirstUser.IsActive!=false) 

					{

						this.LeaveRoom(this.FirstUser.UserID);

					}

				}

				if (this.SecondUser.LastSeen != System.DateTime.MinValue )

				{


					TimeSpan span = DateTime.Now - this.SecondUser.LastSeen;

					if (span.TotalSeconds > window && this.SecondUser.IsActive!=false) 

					{

						this.LeaveRoom(this.SecondUser.UserID);

					}

				}

			}

		}

		/// <summary>

		/// Adds a message to the room

		/// <param name="msg"></param>

		/// <returns> the id of the new message</returns>

		public int AddMsg(Message msg) 

		{

			int count;

			lock(messages) 

			{

				count = messages.Count;

				messages.Add(msg);

			}

			return count;

		}

		/// <summary>

		/// Iterates over the messages array calling ToString() for each message

		/// </summary>

		/// <param name="msgs"></param>

		/// <returns></returns>

		private string GenerateMessagesString(ArrayList msgs)

		{

			string res="";

			for (int i=0;i<msgs.Count;i++)

			{

				res+=((Message)msgs[i]).ToString()+"\n"; 

			}

			return res;

		}

		/// <summary>

		/// Returns an array that contains all messages sent after the message with id=msgid

		/// </summary>

		/// <param name="msgid">The id of the message after which all the message will be retuned </param>

		/// <param name="lastMsgID">the id of the last message returned</param>

		/// <returns></returns>

		public ArrayList GetMessagesSince(int msgid,out int lastMsgID) 

		{

			lock(messages) 

			{

				if ((messages.Count) <= (msgid+1))

					lastMsgID=-1;

				else

					lastMsgID=messages.Count-1;

				return messages.GetRange(msgid+1 , messages.Count - (msgid+1));

			}

		}


		/// <summary>

		/// Returns all the messages sent since the last message the user received

		/// </summary>

		/// <param name="userID"></param>

		/// <returns></returns>

		public string UpdateUser(string userID)

		{

			ChatUser user=this.GetUser(userID);

			user.LastSeen=DateTime.Now;

			this.ExpireUsers(100);

			int lastMsgID;

			ArrayList previousMsgs= this.GetMessagesSince( user.LastMessageReceived,out lastMsgID);

			if (lastMsgID!=-1)

				user.LastMessageReceived=lastMsgID;

			string res=this.GenerateMessagesString(previousMsgs);

			return res;

		}

	}

	#region the ChatUser Class

	public class ChatUser : IDisposable 

	{

		public string UserID;

		public string UserName;

		public bool IsActive;

		public DateTime LastSeen;

		public int LastMessageReceived;

		public ChatUser(string id,string userName)

		{

			this.UserID=id;

			this.IsActive=false;

			this.LastSeen=DateTime.MinValue ;

			this.UserName=userName;

			this.LastMessageReceived=0;

		}

		public void Dispose()

		{

			this.UserID="";

			this.IsActive=false;

			this.LastSeen=DateTime.MinValue ;

			this.UserName="";

			this.LastMessageReceived=0;

		}

	}

	#endregion

	#region the Message Class

	public class Message 

	{

		public string user;

		public string msg;

		public MsgType type;


		public Message(string _user, string _msg, MsgType _type) 

		{

			user = _user;

			msg = _msg;

			type = _type;

		}

		public override string ToString()

		{

			switch(this.type)

			{

				case MsgType.Msg:

					return this.user+" says: "+this.msg;

				case MsgType.Join :

					return this.user + " has joined the room";

				case MsgType.Left :

					return this.user + " has left the room";

			}

			return "";

		}


		public Message(string _user, MsgType _type) : this(_user, "", _type) { }

		public Message(MsgType _type) : this("", "", _type) { }

	}

	public enum MsgType { Msg, Start, Join, Left, Action }

	#endregion

}

