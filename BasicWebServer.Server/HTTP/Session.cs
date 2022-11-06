using BasicWebServer.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebServer.Server.HTTP
{

    public class Session
    {
        private Dictionary<string, string> _data;

        public const string SessionCookieName = "MyWebServerSID";
        public const string SessionCurrentDateKey = "CurrentDate";
        public const string SessionUserKey = "AuthenticatedUserId";


        public Session(string id)
        {
            Guard.AgainstNull(id, nameof(id));

            this.Id = id;

            this._data = new Dictionary<string, string>();
        }

        public string Id { get; init; }

        public string this[string key]
        {
            get => this._data[key];
            set => this._data[key] = value;
        }

        public bool ContainsKey(string key)
          => this._data.ContainsKey(key);

        public void Clear()
            => this._data.Clear();

    }
}
