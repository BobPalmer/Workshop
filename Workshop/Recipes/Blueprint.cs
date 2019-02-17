using System.Collections.Generic;

namespace Workshop.Recipes
{
    using System.Linq;
    using System.Text;
    using UnityEngine;

    public class Blueprint : List<WorkshopResource>, IConfigNode
    {
        public float Funds { get; set; }
        public double Complexity { get; set; }

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

        public string Print(WorkshopUtils.ProductivityType type, double productivity)
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
            if (HighLogic.CurrentGame.Parameters.CustomParams<Workshop_Settings>().useComplexity && type == WorkshopUtils.ProductivityType.printer)
                sb.AppendLine("Complexity: " + Complexity);
            var duration = this.Sum(r => r.Units) / (productivity);

            sb.AppendLine("Base duration: " + KSPUtil.PrintTime(duration, 5, false));
            if (type == WorkshopUtils.ProductivityType.printer)
                duration = this.Sum(r => r.Units) / (productivity / Complexity);
            else
                duration = this.Sum(r => r.Units) / (productivity);

            if (HighLogic.CurrentGame.Parameters.CustomParams<Workshop_Settings>().useComplexity && type == WorkshopUtils.ProductivityType.printer)
                sb.Append("Complexity duration: " + KSPUtil.PrintTime(duration, 5, false));

            return sb.ToString();
        }

        public double GetBuildTime(WorkshopUtils.ProductivityType type, double productivity, double ConversionRate = 1)
        {
            var totalAmount = this.Sum(r => r.Units);
            var totalProcessed = this.Sum(r => r.Processed);
            Log.Info("GetBuildtime, totalAmount: " + totalAmount + ", totalProcessed: " + totalProcessed + ", type: " + type);

            if (type == WorkshopUtils.ProductivityType.printer)
                return (totalAmount - totalProcessed) / (productivity / Complexity) ;
            else
                return (totalAmount * ConversionRate - totalProcessed) / (productivity);
        }

        public void Load(ConfigNode node)
        {
            if (node.HasValue("Funds"))
            {
                Funds = float.Parse(node.GetValue("Funds"));
            }
            if (node.HasValue("Complexity"))
            {
                Complexity = float.Parse(node.GetValue("Complexity"));
            }
            else
                Complexity = 1;
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
            node.AddValue("Complexity", Complexity);
            foreach (var resource in this)
            {
                var n = node.AddNode("WorkshopResource");
                resource.Save(n);
            }
        }
    }
}
