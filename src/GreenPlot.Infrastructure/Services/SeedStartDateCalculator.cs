using GreenPlot.Application.Common.Interfaces;

namespace GreenPlot.Infrastructure.Services;

public class SeedStartDateCalculator : ISeedStartDateCalculator
{
    public SeedStartDates Calculate(
        DateOnly lastFrostDate,
        DateOnly firstFallFrostDate,
        int? weeksBeforeLastFrostMin,
        int? weeksBeforeLastFrostMax,
        int? daysToMaturity)
    {
        const int indoorStartWeeksMin = 6;
        const int indoorStartWeeksMax = 8;
        const int weeksAfterFrostForTransplantMin = 1;
        const int weeksAfterFrostForTransplantMax = 2;

        DateOnly? indoorStartMin = null, indoorStartMax = null;
        DateOnly? transplantMin = null, transplantMax = null;
        DateOnly? directSowMin = null, directSowMax = null;

        indoorStartMin = lastFrostDate.AddDays(-(indoorStartWeeksMax * 7));
        indoorStartMax = lastFrostDate.AddDays(-(indoorStartWeeksMin * 7));
        transplantMin = lastFrostDate.AddDays(weeksAfterFrostForTransplantMin * 7);
        transplantMax = lastFrostDate.AddDays(weeksAfterFrostForTransplantMax * 7);

        directSowMin = weeksBeforeLastFrostMin.HasValue
            ? lastFrostDate.AddDays(-(weeksBeforeLastFrostMin.Value * 7))
            : lastFrostDate.AddDays(-14);

        directSowMax = weeksBeforeLastFrostMax.HasValue
            ? lastFrostDate.AddDays(-(weeksBeforeLastFrostMax.Value * 7))
            : lastFrostDate.AddDays(-7);

        DateOnly? firstHarvest = null;
        if (daysToMaturity.HasValue && transplantMin.HasValue)
            firstHarvest = transplantMin.Value.AddDays(daysToMaturity.Value);

        return new SeedStartDates(
            indoorStartMin, indoorStartMax,
            transplantMin, transplantMax,
            directSowMin, directSowMax,
            firstHarvest);
    }
}
