namespace oa.Models
{
    public class DashboardBundleModel
    {
        public CurrentRaitoModel currentRaitoModel { get; set; }

        public DashboardBundleModel(CurrentRaitoModel currentRaitoModel)
        {
            this.currentRaitoModel = currentRaitoModel;
        }
    }
}
