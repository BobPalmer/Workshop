using System.Collections.Generic;

namespace Workshop.Recipes
{
    using System.Linq;
    using System.Text;
    using UnityEngine;

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

            if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER && WorkshopOptions.PrintingCostsFunds)
            {
                sb.AppendLine("Resource costs: " + ResourceCosts().ToString("N1"));
                sb.AppendLine("Funds: " + Funds);
            }

            var duration = this.Sum(r => r.Units) / productivity;
            
            sb.Append(KSPUtil.PrintTime(duration, 5, false));

            return sb.ToString();
        }

        public double GetBuildTime(double productivity)
        {
            var totalAmount = this.Sum(r => r.Units);
            var totalProcessed = this.Sum(r => r.Processed);
            return (totalAmount - totalProcessed) / productivity;
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
