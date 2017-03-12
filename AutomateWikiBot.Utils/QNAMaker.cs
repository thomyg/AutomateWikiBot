using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomateWikiBot.Utils
{
    public class QnaPair
    {

        [JsonProperty("answer")]
        public string answer { get; set; }

        [JsonProperty("question")]
        public string question { get; set; }
    }

    public class AddToKB
    {

        [JsonProperty("qnaPairs")]
        public IList<QnaPair> qnaPairs { get; set; }

        [JsonProperty("urls")]
        public IList<string> urls { get; set; }
    }

    public class DeleteFromKB
    {

        [JsonProperty("qnaPairs")]
        public IList<QnaPair> qnaPairs { get; set; }

        [JsonProperty("urls")]
        public IList<string> urls { get; set; }
    }

    public class QnaMakerUpdate

    {

        [JsonProperty("add")]
        public AddToKB addToKB { get; set; }

        [JsonProperty("delete")]
        public DeleteFromKB deleteFromKB { get; set; }
    }


}
