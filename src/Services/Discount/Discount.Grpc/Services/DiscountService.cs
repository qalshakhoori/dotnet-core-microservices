﻿using AutoMapper;
using Discount.Grpc.Entities;
using Discount.Grpc.Protos;
using Discount.Grpc.Repositories;
using Grpc.Core;

namespace Discount.Grpc.Services
{
  public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
  {
    private readonly IDiscountRepository _repo;
    private readonly ILogger<DiscountService> _logger;
    private readonly IMapper _mapper;

    public DiscountService(IDiscountRepository repo, ILogger<DiscountService> logger, IMapper mapper)
    {
      _repo = repo;
      _logger = logger;
      _mapper = mapper;
    }

    public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
    {
      var coupon = await _repo.GetDiscount(request.ProductName) ??
        throw new RpcException(new Status(StatusCode.NotFound, $"Discount with ProductName={request.ProductName} was not found"));

      _logger.LogInformation("Discount is retrieved for ProductName : {productName}, Amount : {amount}", coupon.ProductName, coupon.Amount);

      var couponModel = _mapper.Map<CouponModel>(coupon);

      return couponModel;
    }

    public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
    {
      var coupon = _mapper.Map<Coupon>(request.Coupon);

      var res = await _repo.CreateDiscount(coupon);

      if (!res)
        return null;

      _logger.LogInformation("Discount is successfully created. ProductName : {ProductName}", coupon.ProductName);

      return _mapper.Map<CouponModel>(coupon);
    }

    public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
    {
      var coupon = _mapper.Map<Coupon>(request.Coupon);

      var res = await _repo.UpdateDiscount(coupon);

      if(!res)
        return null;

      _logger.LogInformation("Discount is successfully created. ProductName : {ProductName}", coupon.ProductName);

      return _mapper.Map<CouponModel>(coupon);
    }

    public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
    {
      var deleted = await _repo.DeleteDiscount(request.ProductName);

      var response = new DeleteDiscountResponse
      {
        Success = deleted
      };

      return response;
    }
  }
}