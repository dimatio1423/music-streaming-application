using BusinessObjects.Models.ResultModels;
using BusinessObjects.Models.SubscriptionModel.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.SubscriptionServices
{
    public interface ISubscriptionService
    {
        Task<ResultModel> ViewSubscription();
        Task<ResultModel> AddNewSubscription(SubscriptionAddReqModel subscriptionAddReqModel, string token);
        Task<ResultModel> UpdateSubscription(SubscriptionUpdateReqModel subscriptionUpdateReqModel, string token);
        Task<ResultModel> DeleteSubscription(int subscriptionId, string token);
    }
}
