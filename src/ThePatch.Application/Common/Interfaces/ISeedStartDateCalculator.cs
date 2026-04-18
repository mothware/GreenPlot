namespace ThePatch.Application.Common.Interfaces;

public interface ISeedStartDateCalculator
{
    SeedStartDates Calculate(
        DateOnly lastFrostDate,
        DateOnly firstFallFrostDate,
        int? weeksBeforeLastFrostMin,
        int? weeksBeforeLastFrostMax,
        int? daysToMaturity);
}

public record SeedStartDates(
    DateOnly? IndoorStartMin,
    DateOnly? IndoorStartMax,
    DateOnly? OutdoorTransplantMin,
    DateOnly? OutdoorTransplantMax,
    DateOnly? DirectSowMin,
    DateOnly? DirectSowMax,
    DateOnly? EstimatedFirstHarvest
);
