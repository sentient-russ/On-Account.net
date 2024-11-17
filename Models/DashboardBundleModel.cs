namespace oa.Models
{
    public class DashboardBundleModel
    {
        public CurrentRaitoModel currentRaitoModel { get; set; }
        public ReturnOnAssetsModel returnOnAssetsModel { get; set; }

        public DashboardBundleModel(CurrentRaitoModel currentRaitoModel, ReturnOnAssetsModel returnOnAssetsModel)
        {
            this.currentRaitoModel = currentRaitoModel;
            this.returnOnAssetsModel = returnOnAssetsModel;
        }
    }
}
