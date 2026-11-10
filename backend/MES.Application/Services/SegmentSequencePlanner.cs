namespace MesMicroservice.Application.Services;

/// <summary>
/// Builds dynamic production-stage numbers from segment dependencies.
/// The number of stages is determined by data and is not limited by code.
/// </summary>
public static class SegmentSequencePlanner
{
    public static Dictionary<string, int> Calculate(
        IEnumerable<string> segmentIds,
        IEnumerable<KeyValuePair<string, string>> dependencies)
    {
        ArgumentNullException.ThrowIfNull(segmentIds);
        ArgumentNullException.ThrowIfNull(dependencies);

        var orderedSegmentIds = segmentIds
            .Select(NormalizeId)
            .ToList();

        if (orderedSegmentIds.Count == 0)
        {
            throw new InvalidOperationException(
                "An operations definition must contain at least one segment.");
        }

        var duplicateSegmentId = orderedSegmentIds
            .GroupBy(id => id, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault(group => group.Count() > 1)
            ?.Key;

        if (duplicateSegmentId is not null)
        {
            throw new InvalidOperationException(
                $"Segment '{duplicateSegmentId}' is duplicated in the operations definition.");
        }

        var segmentIdSet = orderedSegmentIds.ToHashSet(
            StringComparer.OrdinalIgnoreCase);

        var incomingEdgeCount = orderedSegmentIds.ToDictionary(
            id => id,
            _ => 0,
            StringComparer.OrdinalIgnoreCase);

        var outgoingEdges = orderedSegmentIds.ToDictionary(
            id => id,
            _ => new HashSet<string>(StringComparer.OrdinalIgnoreCase),
            StringComparer.OrdinalIgnoreCase);

        foreach (var dependency in dependencies)
        {
            var fromSegmentId = NormalizeId(dependency.Key);
            var toSegmentId = NormalizeId(dependency.Value);

            if (!segmentIdSet.Contains(fromSegmentId))
            {
                throw new InvalidOperationException(
                    $"Dependency source segment '{fromSegmentId}' does not belong to the operations definition.");
            }

            if (!segmentIdSet.Contains(toSegmentId))
            {
                throw new InvalidOperationException(
                    $"Dependency target segment '{toSegmentId}' does not belong to the operations definition.");
            }

            if (string.Equals(
                    fromSegmentId,
                    toSegmentId,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Segment '{fromSegmentId}' cannot depend on itself.");
            }

            if (!outgoingEdges[fromSegmentId].Add(toSegmentId))
            {
                throw new InvalidOperationException(
                    $"Dependency '{fromSegmentId}' -> '{toSegmentId}' already exists.");
            }

            incomingEdgeCount[toSegmentId]++;
        }

        var currentLevel = new SortedSet<string>(
            orderedSegmentIds.Where(id => incomingEdgeCount[id] == 0),
            StringComparer.OrdinalIgnoreCase);

        var sequenceBySegmentId = new Dictionary<string, int>(
            StringComparer.OrdinalIgnoreCase);

        var sequenceNumber = 1;

        while (currentLevel.Count > 0)
        {
            var nextLevel = new SortedSet<string>(
                StringComparer.OrdinalIgnoreCase);

            foreach (var currentSegmentId in currentLevel)
            {
                sequenceBySegmentId[currentSegmentId] = sequenceNumber;

                foreach (var nextSegmentId in outgoingEdges[currentSegmentId])
                {
                    incomingEdgeCount[nextSegmentId]--;

                    if (incomingEdgeCount[nextSegmentId] == 0)
                    {
                        nextLevel.Add(nextSegmentId);
                    }
                }
            }

            currentLevel = nextLevel;
            sequenceNumber++;
        }

        if (sequenceBySegmentId.Count != orderedSegmentIds.Count)
        {
            var cyclicSegmentIds = orderedSegmentIds
                .Where(id => !sequenceBySegmentId.ContainsKey(id));

            throw new InvalidOperationException(
                $"Circular segment dependency detected: {string.Join(", ", cyclicSegmentIds)}.");
        }

        return sequenceBySegmentId;
    }

    private static string NormalizeId(string? segmentId)
    {
        if (string.IsNullOrWhiteSpace(segmentId))
        {
            throw new InvalidOperationException(
                "Segment IDs in an operations definition and its dependencies cannot be empty.");
        }

        return segmentId.Trim();
    }
}
