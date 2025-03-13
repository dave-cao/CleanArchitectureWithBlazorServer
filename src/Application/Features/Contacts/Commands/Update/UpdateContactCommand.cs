﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This file is part of the CleanArchitecture.Blazor project.
//     Licensed to the .NET Foundation under one or more agreements.
//     The .NET Foundation licenses this file to you under the MIT license.
//     See the LICENSE file in the project root for more information.
//
//     Author: neozhu
//     Created Date: 2024-11-12
//     Last Modified: 2024-11-12
//     Description: 
//       This file defines the UpdateContactCommand and its handler for updating 
//       an existing Contact entity within the CleanArchitecture.Blazor application. 
//       It includes caching invalidation logic to maintain data consistency and 
//       raises a domain event upon successful update to notify other parts of the system.
//     
//     Documentation:
//       https://docs.cleanarchitectureblazor.com/features/contact
// </auto-generated>
//------------------------------------------------------------------------------

// Usage:
// Use `UpdateContactCommand` to update an existing contact entity in the system. 
// The handler ensures that if the entity is found, the changes are applied and 
// the necessary domain event (`ContactUpdatedEvent`) is raised. Caching is also 
// invalidated to keep the contact list consistent.

using CleanArchitecture.Blazor.Application.Features.Contacts.Caching;
using CleanArchitecture.Blazor.Application.Features.Contacts.Commands.Create;

namespace CleanArchitecture.Blazor.Application.Features.Contacts.Commands.Update;

public class UpdateContactCommand: ICacheInvalidatorRequest<Result<int>>
{
      [Description("Id")]
      public int Id { get; set; }
            [Description("Name")]
    public string Name {get;set;} 
    [Description("Description")]
    public string? Description {get;set;} 
    [Description("Email")]
    public string? Email {get;set;} 
    [Description("Phone number")]
    public string? PhoneNumber {get;set;} 
    [Description("Country")]
    public string? Country {get;set;} 

      public string CacheKey => ContactCacheKey.GetAllCacheKey;
      public IEnumerable<string>? Tags => ContactCacheKey.Tags;
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<UpdateContactCommand, Contact>(MemberList.None);
        }
    }
}

public class UpdateContactCommandHandler : IRequestHandler<UpdateContactCommand, Result<int>>
{
    private readonly Mapper _mapper;
    private readonly IApplicationDbContext _context;
    public UpdateContactCommandHandler(
        Mapper mapper,
        IApplicationDbContext context)
    {
        _mapper = mapper;
        _context = context;
    }
    public async Task<Result<int>> Handle(UpdateContactCommand request, CancellationToken cancellationToken)
    {

       var item = await _context.Contacts.FindAsync(request.Id, cancellationToken);
       if (item == null)
       {
           return await Result<int>.FailureAsync($"Contact with id: [{request.Id}] not found.");
       }
        item = _mapper.Map(request, item);
        // raise a update domain event
        item.AddDomainEvent(new ContactUpdatedEvent(item));
       await _context.SaveChangesAsync(cancellationToken);
       return await Result<int>.SuccessAsync(item.Id);
    }
}

