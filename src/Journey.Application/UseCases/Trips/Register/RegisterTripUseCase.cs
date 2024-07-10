using Journey.Communication.Requests;
using Journey.Communication.Responses;
using Journey.Exception.ExceptionsBase;
using Journey.Infrastructure;
using Journey.Infrastructure.Entities;

namespace Journey.Application.UseCases.Trips.Register;

public class RegisterTripUseCase
{
    public ResponseTripJson Execute(RequestRegisterTripJson request)
    {
        Validate(request);

        var dbContext = new JourneyDbContext();

        var entity = new Trip
        {
            Name = request.Name,
            StartDate = request.StartDate,
            EndDate = request.EndDate
        };
        
        dbContext.Trips.Add(entity);
        dbContext.SaveChanges();

        return new ResponseTripJson
        {
            EndDate = entity.EndDate,
            StartDate = entity.StartDate,
            Name = entity.Name,
            Id = entity.Id,
        };
    }

    private void Validate(RequestRegisterTripJson request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new JourneyException("O nome não pode ser vazio");
        }

        if (request.StartDate.Date < DateTime.UtcNow)
        {
            throw new JourneyException("A viagem não pode ser registrada para uma data passada.");
        }

        if (request.EndDate.Date < request.StartDate.Date)
        {
            throw new JourneyException("A viagem deve terminar após a data de início.");
        }
    }
}