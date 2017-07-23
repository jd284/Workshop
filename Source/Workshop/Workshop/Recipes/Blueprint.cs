﻿using System.Collections.Generic;

namespace Workshop.Recipes
{
    using KSP.Localization;
    using System.Linq;
    using System.Text;

    public class Blueprint : List<WorkshopResource>, IConfigNode
    {
        public float Funds { get; set; }

        public double GetProgress()
        {
            var totalAmount = this.Sum(r => r.Units);
            var totalProcessed = this.Sum(r => r.Processed);
            return totalProcessed / totalAmount;
        }

        public double ResourceCosts()
        {
            return this.Sum(r => r.Costs());
        }

        public string Print(double productivity)
        {
            var sb = new StringBuilder();
            foreach (var res in this)
            {
                sb.AppendLine(res.Name + " : " + res.Units.ToString("N1"));
            }

            if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER)
            {
                sb.AppendLine(Localizer.Format("#LOC_Workshop_Blueprint_ResourceCost", ResourceCosts().ToString("N1"))); // Resource costs: <<1>>
                sb.AppendLine(Localizer.Format("#LOC_Workshop_Blueprint_FundsCost", Funds)); // Funds: <<1>>
            }

            var duration = this.Sum(r => r.Units) / productivity;
            sb.AppendFormat(Localizer.Format("#LOC_Workshop_Blueprint_Duration", (duration / 3600).ToString("00"), ((duration / 60) % 60).ToString("00"), (duration % 60).ToString("00"))); // "Duration: {0:00}h {1:00}m {2:00}s"

            return sb.ToString();
        }

        public void Load(ConfigNode node)
        {
            if (node.HasValue("Funds"))
            {
                Funds = float.Parse(node.GetValue("Funds"));
            }
            foreach (var configNode in node.GetNodes("WorkshopResource"))
            {
                var resource = new WorkshopResource();
                resource.Load(configNode);
                Add(resource);
            }
        }

        public void Save(ConfigNode node)
        {
            node.AddValue("Funds", Funds);
            foreach (var resource in this)
            {
                var n = node.AddNode("WorkshopResource");
                resource.Save(n);
            }
        }
    }
}
