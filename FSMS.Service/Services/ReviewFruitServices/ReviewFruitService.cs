﻿using AutoMapper;
using FSMS.Entity.Models;
using FSMS.Entity.Repositories.FruitRepositories;
using FSMS.Entity.Repositories.ReviewFruitRepositories;
using FSMS.Entity.Repositories.UserRepositories;
using FSMS.Service.Enums;
using FSMS.Service.Services.FileServices;
using FSMS.Service.ViewModels.ReviewFruits;

namespace FSMS.Service.Services.ReviewFruitServices
{
    public class ReviewFruitService : IReviewFruitService
    {
        private IUserRepository _userRepository;
        private IFruitRepository _fruitRepository;
        private IReviewFruitRepository _reviewFruitRepository;
        private readonly IFileService _fileService;
        private IMapper _mapper;
        public ReviewFruitService(IUserRepository userRepository, IMapper mapper, IFruitRepository fruitRepository,
            IReviewFruitRepository reviewFruitRepository, IFileService fileService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _fruitRepository = fruitRepository;
            _reviewFruitRepository = reviewFruitRepository;
            _fileService = fileService;
        }
        public async Task CreateReviewFruitAsync(CreateReviewFruit createReviewFruit)
        {
            try
            {

                User existedUser = (await _userRepository.GetByIDAsync(createReviewFruit.UserId));
                if (existedUser == null)
                {
                    throw new Exception("User Id does not exist in the system.");
                }

                Fruit existedFruit = (await _fruitRepository.GetByIDAsync(createReviewFruit.FruitId));
                if (existedFruit == null)
                {
                    throw new Exception("Fruit Id does not exist in the system.");
                }


                int lastId = (await _reviewFruitRepository.GetAsync()).Max(x => x.ReviewId);
                ReviewFruit reviewFruit = new ReviewFruit()
                {

                    ReviewComment = createReviewFruit.ReviewComment,
                    Rating = createReviewFruit.Rating,
                    /*                    ReviewImageUrl = createReviewFruit.ReviewImageUrl,
                    */
                    FruitId = createReviewFruit.FruitId,
                    UserId = createReviewFruit.UserId,
                    Status = StatusEnums.Active.ToString(),
                    CreatedDate = DateTime.Now,
                    ReviewId = lastId + 1,
                    ParentId = createReviewFruit.ParentId
                };
                if (createReviewFruit.UploadFile == null)
                {
                    reviewFruit.ReviewImageUrl = "";
                }
                else if (createReviewFruit.UploadFile != null) reviewFruit.ReviewImageUrl = await _fileService.UploadFile(createReviewFruit.UploadFile);

                await _reviewFruitRepository.InsertAsync(reviewFruit);
                await _reviewFruitRepository.CommitAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task DeleteReviewFruitAsync(int key)
        {
            try
            {
                ReviewFruit existedReviewFruit = await _reviewFruitRepository.GetByIDAsync(key);

                if (existedReviewFruit == null)
                {
                    throw new Exception("ReviewFruit ID does not exist in the system.");
                }
                if (existedReviewFruit.Status != StatusEnums.Active.ToString())
                {
                    throw new Exception("ReviewFruit is not active.");
                }
                existedReviewFruit.Status = StatusEnums.InActive.ToString();

                await _reviewFruitRepository.UpdateAsync(existedReviewFruit);
                await _reviewFruitRepository.CommitAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        public async Task<GetReviewFruit> GetAsync(int key)
        {
            try
            {
                ReviewFruit reviewFruit = await _reviewFruitRepository.GetByIDAsync(key);

                if (reviewFruit == null)
                {
                    throw new Exception("ReviewFruit ID does not exist in the system.");
                }
                if (reviewFruit.Status != StatusEnums.Active.ToString())
                {
                    throw new Exception("reviewFruit is not active.");
                }
                List<GetReviewFruit> reviewFruits = _mapper.Map<List<GetReviewFruit>>(
                  await _reviewFruitRepository.GetAsync(includeProperties: "User,Fruit")
              );

                GetReviewFruit result = _mapper.Map<GetReviewFruit>(reviewFruit);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<List<GetReviewFruit>> GetAllReviewFruitsAsync(bool activeOnly = false, int? fruitId = null)
        {
            try
            {
                var reviewFruits = (await _reviewFruitRepository.GetAsync(includeProperties: "User,Fruit"))
                    .Where(reviewProduct =>
                        (!activeOnly || reviewProduct.Status == StatusEnums.Active.ToString()) &&
                        (!fruitId.HasValue || reviewProduct.FruitId == fruitId.Value));

                List<GetReviewFruit> reviewFruitList = _mapper.Map<List<GetReviewFruit>>(reviewFruits);

                return reviewFruitList;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        public async Task UpdateReviewFruitAsync(int key, UpdateReviewFruit updateReviewFruit)
        {
            try
            {
                ReviewFruit existedReviewFruit = await _reviewFruitRepository.GetByIDAsync(key);

                if (existedReviewFruit == null)
                {
                    throw new Exception("ReviewFruit ID does not exist in the system.");
                }

                if (!string.IsNullOrEmpty(updateReviewFruit.ReviewComment))
                {
                    existedReviewFruit.ReviewComment = updateReviewFruit.ReviewComment;
                }

                /* if (!string.IsNullOrEmpty(updateReviewFruit.ReviewImageUrl))
                 {
                     existedReviewFruit.ReviewImageUrl = updateReviewFruit.ReviewImageUrl;
                 }*/

                if (updateReviewFruit.UploadFile != null)
                {
                    existedReviewFruit.ReviewImageUrl = await _fileService.UploadFile(updateReviewFruit.UploadFile);
                }


                if (!string.IsNullOrEmpty(updateReviewFruit.Status))
                {
                    if (updateReviewFruit.Status != "Active" && updateReviewFruit.Status != "InActive")
                    {
                        throw new Exception("Status must be 'Active' or 'InActive'.");
                    }
                    existedReviewFruit.Status = updateReviewFruit.Status;
                }

                existedReviewFruit.Rating = updateReviewFruit.Rating;

                existedReviewFruit.UpdateDate = DateTime.Now;


                await _reviewFruitRepository.UpdateAsync(existedReviewFruit);
                await _reviewFruitRepository.CommitAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }
    }
}
