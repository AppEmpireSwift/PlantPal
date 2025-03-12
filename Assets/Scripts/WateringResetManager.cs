using System;
using System.Collections.Generic;
using MainScreen;
using Plant;

public static class WateringResetManager
{
    public static void CheckAndResetWateringProgress(List<PlantPlane> plantPlanes)
    {
        DateTime currentDate = DateTime.Now;

        foreach (var plantPlane in plantPlanes)
        {
            if (plantPlane.IsActive && plantPlane.PlantData != null)
            {
                bool shouldReset = ShouldResetProgress(plantPlane.PlantData, currentDate);

                if (shouldReset)
                {
                    plantPlane.PlantData.WateringProgress = 0;
                    plantPlane.PlantData.LastWateringResetDate = currentDate;

                    plantPlane.UpdateWateringCount(0);
                }
            }
        }
    }

    private static bool ShouldResetProgress(PlantData plantData, DateTime currentDate)
    {
        if (plantData == null)
            return false;

        DateTime lastResetDate = plantData.LastWateringResetDate;
        TimeSpan timeSinceLastReset = currentDate - lastResetDate;

        switch (plantData.WateringType)
        {
            case WateringType.Daily:
                return currentDate.Date > lastResetDate.Date;

            case WateringType.EveryTwoDays:
                return timeSinceLastReset.TotalDays >= 2;

            case WateringType.TwiceAWeek:
                return timeSinceLastReset.TotalDays >= 3;

            case WateringType.OnceAWeek:
                return timeSinceLastReset.TotalDays >= 7;

            case WateringType.EveryTwoWeeks:
                return timeSinceLastReset.TotalDays >= 14;

            case WateringType.OnceAMonth:
                return timeSinceLastReset.TotalDays >= 30;

            case WateringType.Custom:
                return timeSinceLastReset.TotalDays >= plantData.CustomFrequency;

            default:
                return false;
        }
    }

    public static void UpdateLastWateringDate(PlantData plantData)
    {
        if (plantData != null)
        {
            plantData.LastWateringDate = DateTime.Now;
        }
    }
}