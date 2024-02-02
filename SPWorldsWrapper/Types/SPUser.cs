using Microsoft.VisualBasic;
using SPWorldsWrapper.Types.UserTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPWorldsWrapper.Types
{


    public class SPUser
    {
        public string id { get; set; }
        public bool isBanned { get; set; }
        public User user { get; set; }
        public string[] roles { get; set; }
        public City? city { get; set; }
        public string status { get; set; }
        public DateTime createdAt { get; set; }
        public List<CardsOwned> cardsOwned { get; set; }
        public bool isFollowed { get; set; }
        public bool isFollowingYou { get; set; }

        public Dictionary<string, object> toKeyValuePairs()
        {
            string cards = "[\n";
            foreach (var card in cardsOwned)
            {
                cards += "      {\n";   
                foreach (var kvp in card.toKeyValuePairs())
                {
                    cards += $"          {kvp.Key}: {kvp.Value},\n";
                }
                cards += "      },\n";
            }
            cards += "]";
            return new ()
            {
                { "id", id },
                { "isBanned", isBanned },
                { "status", status },
                { "created_at", createdAt },
                { "isFollowed", isFollowed },
                { "isFollowingYou", isFollowingYou },
                { "user", user.ToString() },
                { "roles", $"[{string.Join(", ", roles)}]" },
                { "city", city?.ToString() ?? "Null" },
                { "cardsOwner", cards },
            };
        }

        public override string ToString()
        {
            string stringToReturn = "{\n";
            foreach (var kvp in toKeyValuePairs())
            {
                stringToReturn += $"\n    {kvp.Key}: {kvp.Value},";
            }
            stringToReturn += "\n}";
            return stringToReturn;
        }

    }
}
