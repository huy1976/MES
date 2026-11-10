using System.Globalization;

namespace MesMicroservice.Algorithm.Services;

/// <summary>Configured elapsed time per work order, expressed internally in hours.</summary>
public static class SegmentDuration
{
    public static double? GetHours(string? segmentType, string? value, string? unit, decimal quantity = 1)
    {
        var legacy = segmentType?.Trim().ToLowerInvariant() is
            "drawing" or "enameling" or "both" or "0" or "1" or "3";

        if (string.IsNullOrWhiteSpace(value) || value.Trim() is "--" or "0")
        {
            if (legacy) return null;
            throw new InvalidOperationException("A new segment type requires a positive duration per work order.");
        }

        if (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var duration)
            || !double.IsFinite(duration) || duration < 0)
            throw new InvalidOperationException("Segment duration must be a finite positive number.");

        if (duration == 0 && legacy) return null;
        if (duration == 0)
            throw new InvalidOperationException("A new segment type requires a positive duration per work order.");

        var normalizedUnit = unit?.Trim().ToLowerInvariant();
        var perUnit = normalizedUnit?.EndsWith("/unit", StringComparison.Ordinal) == true;
        if (perUnit)
        {
            if (quantity <= 0)
                throw new InvalidOperationException("A per-unit duration requires a positive production quantity.");
            normalizedUnit = normalizedUnit![..^5];
            duration *= (double)quantity;
        }

        var hours = normalizedUnit switch
        {
            "h" or "hour" or "hours" or "giờ" => duration,
            "min" or "minute" or "minutes" or "phút" => duration / 60.0,
            "s" or "second" or "seconds" or "giây" => duration / 3600.0,
            _ => throw new InvalidOperationException("Segment duration unit must be h, min, s, h/unit, min/unit or s/unit.")
        };
        if (!double.IsFinite(hours) || hours <= 0)
            throw new InvalidOperationException("Segment duration is outside the supported range.");
        return hours;
    }
}
