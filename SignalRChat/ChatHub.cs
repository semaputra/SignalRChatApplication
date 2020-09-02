using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using System.Web.Security;

namespace SignalRChat {
    public class ChatHub : Hub {
        private readonly static ConnectionMapping<string> _connections =
            new ConnectionMapping<string>();
        private static Dictionary<string, string> users = new Dictionary<string, string>();
        private static string id;
        private static List<List<string>> messages = new List<List<string>>();


        public void Connect(string name) {        
            users.Add(id, name);
            Clients.All.onConnected(users);
            Clients.All.makePrivateChatWindow(users);
        }

        public string getId() {
            id = Context.ConnectionId;
            return id;
        }

        public void Send(string name, string message, string time)
        {
            List<string> messageInfo = new List<string>() { name, message, time };
            messages.Add(messageInfo);
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(name, message, time);
        }

        public void getMessages() {
            Clients.Caller.getMessages(messages);
        }

        public void sendPrivateMessage(string toId, string idFrom, string nameFrom, string message) {
            Clients.Client(toId).sendPrivateMessage(toId, idFrom, nameFrom, message);
            Clients.Caller.sendPrivateMessage(toId, idFrom, nameFrom, message);
        }

        //public override Task OnConnected() {
        //    string id = Context.ConnectionId;
        //    string name = Context.User.Identity.Name;
        //    _connections.Add(name, id);
        //    IEnumerable<string> userList = _connections.GetConnections(name);
        //    Clients.All.onConnected(id, name, userList);
        //    return base.OnConnected();
        //}

        public override Task OnDisconnected(bool stopCalled) {
            string removedId = Context.ConnectionId;
            users.Remove(removedId);
            Clients.AllExcept(removedId).onDisconnected(removedId);
            return base.OnDisconnected(stopCalled);
        }

        //public override Task OnReconnected() {
        //    string name = Context.User.Identity.Name;
        //    if (!_connections.GetConnections(name).Contains(Context.ConnectionId)) {
        //        _connections.Add(name, Context.ConnectionId);
        //    }
        //    return base.OnReconnected();
        //}
    }


}