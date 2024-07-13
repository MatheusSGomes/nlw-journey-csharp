using Journey.Communication.Requests;
using Journey.Communication.Responses;
using Journey.Exception.ExceptionsBase;
using Journey.Infrastructure;
using Journey.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using FluentValidation.Results;

namespace Journey.Application.UseCases.Activities.Register;

public class RegisterActivityForTripUseCase
{
    public ResponseActivityJson Execute(Guid tripId, RequestRegisterActivityJson request)
    {
        var dbContext = new JourneyDbContext();

        var trip = dbContext
            .Trips
            .Include(trip => trip.Activities) // Não necessário quando usado o SqLite
            .FirstOrDefault(trip => trip.Id == tripId);

        Validate(trip, request);

        var entity = new Activity
        {
            Name = request.Name,
            Date = request.Date,
            TripId = tripId, // Necessário apenas com o SqLite
        };

        dbContext.Activities.Add(entity);

        // Não funciona com SqLite
        // trip!.Activities.Add(entity);
        // dbContext.Trips.Update(trip);

        dbContext.SaveChanges();

        return new ResponseActivityJson
        {
            Id = entity.Id,
            Name = entity.Name,
            Date = entity.Date,
            Status = (Communication.Enums.ActivityStatus) entity.Status
        };
    }

    private void Validate(Trip? trip, RequestRegisterActivityJson request)
    {
        if (trip is null)
        {
            throw new NotFoundException(ResourceErrorMessages.TRIP_NOT_FOUND);
        }

        var validator = new RegisterActivityValidator();
        var result = validator.Validate(request);

        if ((request.Date >= trip.StartDate && request.Date <= trip.EndDate) == false)
        {
            var error = new ValidationFailure("Date", ResourceErrorMessages.DATE_NOT_WITHIN_TRAVEL_PERIOD);
            result.Errors.Add(error);
        }

        if (result.IsValid == false)
        {
            var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }
}