using AutoMapper;
using BusinessObjects.Entities;
using BusinessObjects.Enums;
using BusinessObjects.Models.ResultModels;
using BusinessObjects.Models.SubscriptionModel.Request;
using BusinessObjects.Models.SubscriptionModel.Response;
using Repositories.SubscriptionRepos;
using Repositories.SubscriptionUserRepos;
using Repositories.UserRepos;
using Services.Helpers.Handler.DecodeTokenHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services.SubscriptionServices
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IUserSubscriptionRepository _userSubscriptionRepository;
        private readonly IDecodeTokenHandler _decodeToken;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public SubscriptionService(ISubscriptionRepository subscriptionRepository, 
            IUserSubscriptionRepository userSubscriptionRepository,
            IDecodeTokenHandler decodeToken, 
            IMapper mapper,
            IUserRepository userRepository)
        {
            _subscriptionRepository = subscriptionRepository;
            _userSubscriptionRepository = userSubscriptionRepository;
            _decodeToken = decodeToken;
            _userRepository = userRepository;
            _mapper = mapper;
        }
        public async Task<ResultModel> AddNewSubscription(SubscriptionAddReqModel subscriptionAddReqModel, string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Add new subscription successfully"
            };

            try
            {
                var decodedToken = _decodeToken.decode(token);

                var currUser = await _userRepository.GetUserByEmail(decodedToken.email);

                if (currUser == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "User does not exist";

                    return result;
                }

                if (!currUser.Role.Equals(RoleEnums.Admin.ToString()))
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.BadRequest;
                    result.Message = "Do not have permission to perform this function";

                    return result;
                }

                var currSubscription = await _subscriptionRepository.GetSubscriptionByName(subscriptionAddReqModel.Name);

                if (currSubscription == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.BadRequest;
                    result.Message = $"Subscription with name {subscriptionAddReqModel.Name} already existed";

                    return result;
                }

                Subscription subscription = new Subscription
                {
                    Name = subscriptionAddReqModel.Name,
                    Price = subscriptionAddReqModel.Price,
                    DurationInDays = subscriptionAddReqModel.DurationInDays,
                    Description = subscriptionAddReqModel.Description,
                    Status = StatusEnums.Active.ToString()
                };

                await _subscriptionRepository.Insert(subscription);


            }
            catch(Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.BadRequest;
                result.Message = ex.Message;
                return result;
            }

            return result;
        }

        public async Task<ResultModel> DeleteSubscription(int subscriptionId, string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Update subscription successfully"
            };

            try
            {
                var decodedToken = _decodeToken.decode(token);

                var currUser = await _userRepository.GetUserByEmail(decodedToken.email);

                if (currUser == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "User does not exist";

                    return result;
                }

                if (!currUser.Role.Equals(RoleEnums.Admin.ToString()))
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.BadRequest;
                    result.Message = "Do not have permission to perform this function";

                    return result;
                }

                var currDeletingSubscription = await _subscriptionRepository.Get(subscriptionId);

                if (currDeletingSubscription == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Subscription not found";

                    return result;
                }

                //var userSubscriptions = await _userSubscriptionRepository.GetUserSubscriptionsBySubscriptionId(currUpdatingSubscription.SubscriptionId);

                //var activeUserSubscription = userSubscriptions.Where(x => x.IsActive).ToList();

                //if (activeUserSubscription.Count > 0)
                //{
                //    result.IsSuccess = false;
                //    result.Code = (int)HttpStatusCode.BadRequest;
                //    result.Message = "The current subscription is currently being used";

                //    return result;
                //}

                currDeletingSubscription.Status = StatusEnums.Inactive.ToString();

                await _subscriptionRepository.Update(currDeletingSubscription);
                
            } catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.BadRequest;
                result.Message = ex.Message;
                return result;
            }
            return result;
         }

        public async Task<ResultModel> UpdateSubscription(SubscriptionUpdateReqModel subscriptionUpdateReqModel, string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Update subscription successfully"
            };

            try
            {
                var decodedToken = _decodeToken.decode(token);

                var currUser = await _userRepository.GetUserByEmail(decodedToken.email);

                if (currUser == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "User does not exist";

                    return result;
                }

                if (!currUser.Role.Equals(RoleEnums.Admin.ToString()))
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.BadRequest;
                    result.Message = "Do not have permission to perform this function";

                    return result;
                }

                var currUpdatingSubscription = await _subscriptionRepository.Get(subscriptionUpdateReqModel.SubscriptionID);

                if (currUpdatingSubscription == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Subscription not found";

                    return result;
                }

                if (!string.IsNullOrEmpty(subscriptionUpdateReqModel.Name) && !currUpdatingSubscription.Name.ToLower().Equals(subscriptionUpdateReqModel.Name.ToLower()))
                {
                    var subscription = await _subscriptionRepository.GetSubscriptionByName(subscriptionUpdateReqModel.Name);
                    if (subscription != null)
                    {
                        result.IsSuccess = false;
                        result.Code = (int)HttpStatusCode.BadRequest;
                        result.Message = $"Subscription with name {subscriptionUpdateReqModel.Name} already existed";

                        return result;
                    }
                }

                currUpdatingSubscription.Name = !string.IsNullOrEmpty(subscriptionUpdateReqModel.Name) ? subscriptionUpdateReqModel.Name : currUpdatingSubscription.Name;
                currUpdatingSubscription.Price = subscriptionUpdateReqModel.Price != null ? (decimal) subscriptionUpdateReqModel.Price : currUpdatingSubscription.Price;
                currUpdatingSubscription.DurationInDays = subscriptionUpdateReqModel.DurationInDays != null ? (int)subscriptionUpdateReqModel.DurationInDays : currUpdatingSubscription.DurationInDays;
                currUpdatingSubscription.Description = !string.IsNullOrEmpty(subscriptionUpdateReqModel.Description) ? subscriptionUpdateReqModel.Description : currUpdatingSubscription.Description;


                await _subscriptionRepository.Update(currUpdatingSubscription);


            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.BadRequest;
                result.Message = ex.Message;
                return result;
            }

            return result;
        }

        public async Task<ResultModel> ViewDetailsSubscription(int subscriptionId)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "View details subscription successfully"
            };

            try
            {
                var subscriptions = await _subscriptionRepository.Get(subscriptionId);
                result.Data = _mapper.Map<SubscriptionViewResModel>(subscriptions);

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.BadRequest;
                result.Message = ex.Message;
                return result;
            }


            return result;
        }

        public async Task<ResultModel> ViewSubscription(string? filterBy)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "View subscriptions successfully"
            };

            try
            {
                var subscriptions = await _subscriptionRepository.GetAll(null, null);

                switch (filterBy)
                {
                    case "Active":
                        result.Data = _mapper.Map<List<SubscriptionViewResModel>>(subscriptions.Select(x => x.Status.Equals(StatusEnums.Active.ToString())).ToList());
                        break;
                    case "Inactive":
                        result.Data = _mapper.Map<List<SubscriptionViewResModel>>(subscriptions.Select(x => x.Status.Equals(StatusEnums.Inactive.ToString())).ToList());
                        break;
                    default:
                        result.Data = _mapper.Map<List<SubscriptionViewResModel>>(subscriptions);
                        break;
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.BadRequest;
                result.Message = ex.Message;
                return result;
            }


            return result;
        }
    }
}
