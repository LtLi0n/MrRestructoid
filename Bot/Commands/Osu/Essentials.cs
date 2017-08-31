using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MrRestructoid.Bot.Commands.Osu
{
    public class osuPlayerRaw
    {
        public string user_id { get; set; }
        public string username { get; set; }
        public string count300 { get; set; }
        public string count100 { get; set; }
        public string count50 { get; set; }
        public string playcount { get; set; }
        public string ranked_score { get; set; }
        public string total_score { get; set; }
        public string pp_rank { get; set; }
        public string level { get; set; }
        public string pp_raw { get; set; }
        public string accuracy { get; set; }
        public string count_rank_ss { get; set; }
        public string count_rank_s { get; set; }
        public string count_rank_a { get; set; }
        public string country { get; set; }
        public string pp_country_rank { get; set; }

        [JsonConstructor]
        public osuPlayerRaw()
        {
            if (string.IsNullOrEmpty(user_id)) user_id = "";
            if (string.IsNullOrEmpty(playcount)) playcount = "0";
            if (string.IsNullOrEmpty(ranked_score)) ranked_score = "0";
            if (string.IsNullOrEmpty(total_score)) total_score = "0";
            if (string.IsNullOrEmpty(pp_rank)) pp_rank = "---";
            if (string.IsNullOrEmpty(level)) level = "0";
            if (string.IsNullOrEmpty(pp_rank)) pp_rank = "---";
            if (string.IsNullOrEmpty(pp_raw)) pp_raw = "0";
            if (string.IsNullOrEmpty(accuracy)) accuracy = "0";
            if (string.IsNullOrEmpty(country)) country = "NULL";
            if (string.IsNullOrEmpty(pp_country_rank)) pp_country_rank = "---";
        }
    }

    public class OsuLevelConverter
    {
        ulong _level = 1;
        ulong _xp = 0, _xpCap = 100;
        double _pp;

        public ulong level { get { return _level; } }
        public ulong xp { get { return _xp; } }
        public ulong xpCap { get { return _xpCap; } }

        public string returnXp, returnXpCap, percentProgress;

        public OsuLevelConverter(uint HitCount, double pp)
        {
            _xp = (uint)(HitCount * (1 + (pp / 1000.0)));

            ConvertToLevel();
            _pp = pp;
        }

        void ConvertToLevel()
        {
            while (_xp >= _xpCap)
            {
                _xp -= xpCap;
                _level++;
                _xpCap += _level * 300;
            }

            float oneHundreth = _xpCap / 100;
            float percentProgressNum = _xp / oneHundreth;

            percentProgress = percentProgressNum.ToString();
            percentProgress = string.Format("{0:0.00}", percentProgressNum);

            returnXp = _xp.ToString("#,##0");
            returnXpCap = _xpCap.ToString("#,##0");
        }
    }
}
