using System;
using System.Collections.Generic;
using System.Linq;
namespace LibGameAI.PCG
{
    public static class TerrainMetrics
    {
        public static float ComputeRoughnessIndex(float[,] heightMap)
        {
            int rows = heightMap.GetLength(0);
            int cols = heightMap.GetLength(1);

            List<float> differences = new List<float>();

            // Directions for 8-neighbor connectivity
            int[] dx = { -1, -1, -1, 0, 0, 1, 1, 1 };
            int[] dy = { -1, 0, 1, -1, 1, -1, 0, 1 };

            for (int x = 1; x < rows - 1; x++)
            {
                for (int y = 1; y < cols - 1; y++)
                {
                    float center = heightMap[x, y];

                    for (int i = 0; i < 8; i++)
                    {
                        int nx = x + dx[i];
                        int ny = y + dy[i];

                        float neighbor = heightMap[nx, ny];
                        float diff = Math.Abs(center - neighbor);
                        differences.Add(diff);
                    }
                }
            }

            return StandardDeviation(differences);
        }

        private static float StandardDeviation(List<float> values)
        {
            if (values.Count == 0) return 0f;

            float mean = 0f;
            foreach (float v in values)
                mean += v;
            mean /= values.Count;

            float sumSq = 0f;
            foreach (float v in values)
                sumSq += (v - mean) * (v - mean);

            return (float)Math.Sqrt(sumSq / values.Count);
        }

        public static float ComputeFractalDimension(float[,] heightMap, int[] boxSizes = null)
        {
            int rows = heightMap.GetLength(0);
            int cols = heightMap.GetLength(1);

            // Default box sizes: powers of 2, fitting inside the heightmap
            if (boxSizes == null)
            {
                int minSize = 2;
                int maxSize = Math.Min(rows, cols) / 2;
                List<int> sizes = new List<int>();
                for (int s = minSize; s <= maxSize; s *= 2)
                    sizes.Add(s);
                boxSizes = sizes.ToArray();
            }

            List<double> logBoxSizes = new List<double>();
            List<double> logCounts = new List<double>();

            foreach (int size in boxSizes)
            {
                int count = CountNonUniformBoxes(heightMap, size);
                if (count > 0)
                {
                    logBoxSizes.Add(Math.Log(1.0 / size));
                    logCounts.Add(Math.Log(count));
                }
            }

            return (float)(LinearRegressionSlope(logBoxSizes, logCounts));
        }

        private static int CountNonUniformBoxes(float[,] map, int boxSize)
        {
            int rows = map.GetLength(0);
            int cols = map.GetLength(1);
            int count = 0;

            for (int x = 0; x < rows; x += boxSize)
            {
                for (int y = 0; y < cols; y += boxSize)
                {
                    bool uniform = true;
                    float? firstValue = null;

                    for (int i = 0; i < boxSize && x + i < rows; i++)
                    {
                        for (int j = 0; j < boxSize && y + j < cols; j++)
                        {
                            float val = map[x + i, y + j];
                            if (firstValue == null)
                                firstValue = val;
                            else if (Math.Abs(val - firstValue.Value) > 0.0001f)
                            {
                                uniform = false;
                                break;
                            }
                        }
                        if (!uniform) break;
                    }

                    if (!uniform)
                        count++;
                }
            }

            return count;
        }

        private static double LinearRegressionSlope(List<double> x, List<double> y)
        {
            int n = x.Count;
            if (n < 2) return 0;

            double sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0;
            for (int i = 0; i < n; i++)
            {
                sumX += x[i];
                sumY += y[i];
                sumXY += x[i] * y[i];
                sumX2 += x[i] * x[i];
            }

            double numerator = n * sumXY - sumX * sumY;
            double denominator = n * sumX2 - sumX * sumX;

            if (denominator == 0) return 0;
            return numerator / denominator;
        }

        public static float ComputeEntropy(float[,] heightMap, int numBins = 256)
        {
            int rows = heightMap.GetLength(0);
            int cols = heightMap.GetLength(1);
            int totalCells = rows * cols;

            // Step 1: Find min and max height
            float min = float.MaxValue;
            float max = float.MinValue;

            foreach (float h in heightMap)
            {
                if (h < min) min = h;
                if (h > max) max = h;
            }

            if (Math.Abs(max - min) < 1e-6f)
                return 0f; // Flat terrain — no entropy

            // Step 2: Build histogram using bins
            int[] histogram = new int[numBins];
            float binSize = (max - min) / numBins;

            foreach (float h in heightMap)
            {
                int bin = (int)((h - min) / binSize);
                if (bin == numBins) bin = numBins - 1; // edge case
                histogram[bin]++;
            }

            // Step 3: Calculate entropy
            float entropy = 0f;
            foreach (int count in histogram)
            {
                if (count == 0) continue;
                float p = (float)count / totalCells;
                entropy -= p * (float)Math.Log(p, 2); // Shannon entropy
            }


            entropy /= (float)Math.Log(numBins, 2);

            return entropy;
        }

        /// <summary>
        /// Computes a walkability map based on slope threshold.
        /// </summary>
        /// <param name="heightMap">The terrain heightmap as a 2D float array.</param>
        /// <param name="maxSlopeDegrees">Maximum slope (in degrees) an agent can traverse.</param>
        /// <param name="cellSize">Physical size of a cell in the same units as height (e.g., meters).</param>
        /// <returns>A bool[,] mask: true for walkable, false for not.</returns>
        public static (float, bool[,]) ComputeSlopeWalkability(float[,] heightMap, float maxSlopeDegrees, float cellSize = 1.0f)
        {
            int rows = heightMap.GetLength(0);
            int cols = heightMap.GetLength(1);
            bool[,] walkable = new bool[rows, cols];

            float maxSlopeTan = (float)Math.Tan(maxSlopeDegrees * Math.PI / 180.0); // Convert degrees to tangent

            int[] dx = { -1, -1, -1, 0, 0, 1, 1, 1 };
            int[] dy = { -1, 0, 1, -1, 1, -1, 0, 1 };

            for (int x = 1; x < rows - 1; x++)
            {
                for (int y = 1; y < cols - 1; y++)
                {
                    bool isWalkable = true;
                    float centerHeight = heightMap[x, y];

                    for (int i = 0; i < 8; i++)
                    {
                        int nx = x + dx[i];
                        int ny = y + dy[i];
                        float neighborHeight = heightMap[nx, ny];

                        float dh = Math.Abs(centerHeight - neighborHeight);
                        float distance = (dx[i] == 0 || dy[i] == 0) ? cellSize : cellSize * (float)Math.Sqrt(2);
                        float slope = dh / distance;

                        if (slope > maxSlopeTan)
                        {
                            isWalkable = false;
                            break;
                        }
                    }

                    walkable[x, y] = isWalkable;
                }
            }

            // Optionally, leave edges as unwalkable
            for (int i = 0; i < rows; i++) { walkable[i, 0] = false; walkable[i, cols - 1] = false; }
            for (int j = 0; j < cols; j++) { walkable[0, j] = false; walkable[rows - 1, j] = false; }

            return (walkable.OfType<bool>().Count(w => w) / (float)walkable.Length, walkable);
        }

        // public static float ComputeSlopeWalkabilityPercentage(float[,] heightMap, float maxSlopeDegrees, float cellSize = 1.0f)
        // {
        //     bool[,] walkable = ComputeSlopeWalkability(heightMap, maxSlopeDegrees, cellSize);
        //     int numWalkable = walkable.OfType<bool>().Count(w => w);
        //     return numWalkable / (float)walkable.Length;

        // }

        public static float ComputePathCoveragePercentage(bool[,] walkableMap)
        {
            int rows = walkableMap.GetLength(0);
            int cols = walkableMap.GetLength(1);
            bool[,] visited = new bool[rows, cols];

            int totalWalkable = 0;
            int largestRegionSize = 0;

            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < cols; y++)
                {
                    if (walkableMap[x, y])
                        totalWalkable++;

                    if (walkableMap[x, y] && !visited[x, y])
                    {
                        int regionSize = FloodFill(walkableMap, visited, x, y);
                        if (regionSize > largestRegionSize)
                            largestRegionSize = regionSize;
                    }
                }
            }

            if (totalWalkable == 0)
                return 0f;

            return (float)largestRegionSize / totalWalkable;
        }

        private static int FloodFill(bool[,] map, bool[,] visited, int startX, int startY)
        {
            int rows = map.GetLength(0);
            int cols = map.GetLength(1);
            int size = 0;

            Queue<(int x, int y)> queue = new Queue<(int x, int y)>();
            queue.Enqueue((startX, startY));
            visited[startX, startY] = true;

            int[] dx = { -1, 0, 1, 0, -1, -1, 1, 1 }; // 8 directions
            int[] dy = { 0, -1, 0, 1, -1, 1, -1, 1 };

            while (queue.Count > 0)
            {
                var (x, y) = queue.Dequeue();
                size++;

                for (int i = 0; i < 8; i++)
                {
                    int nx = x + dx[i];
                    int ny = y + dy[i];

                    if (nx >= 0 && nx < rows && ny >= 0 && ny < cols &&
                        map[nx, ny] && !visited[nx, ny])
                    {
                        visited[nx, ny] = true;
                        queue.Enqueue((nx, ny));
                    }
                }
            }

            return size;
        }

        public static float AveragePathCost(float[,] heightMap, bool[,] walkableMap, int sampleSize = 100, float slopeCostFactor = 10f)
        {
            var regionPoints = GetLargestWalkableRegion(walkableMap);
            if (regionPoints.Count < 2) return 0f;

            Random rand = new Random();
            int rows = heightMap.GetLength(0);
            int cols = heightMap.GetLength(1);

            float totalCost = 0f;
            int successfulPaths = 0;

            for (int i = 0; i < sampleSize; i++)
            {
                var start = regionPoints[rand.Next(regionPoints.Count)];
                var end = regionPoints[rand.Next(regionPoints.Count)];
                if (start == end) continue;

                float cost = AStarCost(heightMap, walkableMap, start, end, slopeCostFactor);
                if (cost >= 0)
                {
                    totalCost += cost;
                    successfulPaths++;
                }
            }

            return successfulPaths > 0 ? totalCost / successfulPaths : 0f;
        }

        public static List<(int x, int y)> GetLargestWalkableRegion(bool[,] walkableMap)
        {
            int rows = walkableMap.GetLength(0);
            int cols = walkableMap.GetLength(1);
            bool[,] visited = new bool[rows, cols];
            List<(int x, int y)> largestRegion = new();

            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < cols; y++)
                {
                    if (walkableMap[x, y] && !visited[x, y])
                    {
                        var region = new List<(int, int)>();
                        FloodFillCollect(walkableMap, visited, x, y, region);

                        if (region.Count > largestRegion.Count)
                            largestRegion = region;
                    }
                }
            }

            return largestRegion;
        }

        private static void FloodFillCollect(bool[,] map, bool[,] visited, int startX, int startY, List<(int, int)> output)
        {
            int rows = map.GetLength(0);
            int cols = map.GetLength(1);
            Queue<(int x, int y)> queue = new();
            queue.Enqueue((startX, startY));
            visited[startX, startY] = true;

            int[] dx = { -1, 0, 1, 0, -1, -1, 1, 1 };
            int[] dy = { 0, -1, 0, 1, -1, 1, -1, 1 };

            while (queue.Count > 0)
            {
                var (x, y) = queue.Dequeue();
                output.Add((x, y));

                for (int i = 0; i < 8; i++)
                {
                    int nx = x + dx[i];
                    int ny = y + dy[i];
                    if (nx >= 0 && nx < rows && ny >= 0 && ny < cols &&
                        map[nx, ny] && !visited[nx, ny])
                    {
                        visited[nx, ny] = true;
                        queue.Enqueue((nx, ny));
                    }
                }
            }
        }
        private static float AStarCost(float[,] heightMap, bool[,] walkableMap, (int x, int y) start, (int x, int y) goal, float slopeFactor)
        {
            int rows = heightMap.GetLength(0);
            int cols = heightMap.GetLength(1);
            float[,] cost = new float[rows, cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    cost[i, j] = float.MaxValue;

            var cmp = Comparer<(float f, int x, int y)>.Create((a, b) => a.f != b.f ? a.f.CompareTo(b.f) : (a.x != b.x ? a.x.CompareTo(b.x) : a.y.CompareTo(b.y)));
            var queue = new SortedSet<(float f, int x, int y)>(cmp);

            cost[start.x, start.y] = 0;
            queue.Add((Heuristic(start, goal), start.x, start.y));

            int[] dx = { -1, 0, 1, 0, -1, -1, 1, 1 };
            int[] dy = { 0, -1, 0, 1, -1, 1, -1, 1 };

            while (queue.Count > 0)
            {
                var (_, x, y) = queue.Min;
                queue.Remove(queue.Min);

                if ((x, y) == goal)
                    return cost[x, y];

                for (int i = 0; i < 8; i++)
                {
                    int nx = x + dx[i];
                    int ny = y + dy[i];
                    if (nx < 0 || nx >= rows || ny < 0 || ny >= cols) continue;
                    if (!walkableMap[nx, ny]) continue;

                    float h1 = heightMap[x, y];
                    float h2 = heightMap[nx, ny];
                    float elevationDiff = Math.Abs(h2 - h1);
                    float baseCost = (dx[i] == 0 || dy[i] == 0) ? 1f : 1.4142f;

                    float moveCost = baseCost + slopeFactor * elevationDiff;
                    float newCost = cost[x, y] + moveCost;

                    if (newCost < cost[nx, ny])
                    {
                        queue.Remove((cost[nx, ny] + Heuristic((nx, ny), goal), nx, ny));
                        cost[nx, ny] = newCost;
                        queue.Add((newCost + Heuristic((nx, ny), goal), nx, ny));
                    }
                }
            }

            return -1f; // No path found
        }

        private static float Heuristic((int x, int y) a, (int x, int y) b)
        {
            int dx = a.x - b.x;
            int dy = a.y - b.y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        public static float AveragePathStretchFactor(float[,] heightMap, bool[,] walkableMap, int sampleSize = 100, float slopeCostFactor = 10f)
        {
            var regionPoints = GetLargestWalkableRegion(walkableMap);
            if (regionPoints.Count < 2) return 0f;

            Random rand = new Random();
            int rows = heightMap.GetLength(0);
            int cols = heightMap.GetLength(1);

            float totalStretch = 0f;
            int validPaths = 0;

            for (int i = 0; i < sampleSize; i++)
            {
                var start = regionPoints[rand.Next(regionPoints.Count)];
                var end = regionPoints[rand.Next(regionPoints.Count)];
                if (start == end) continue;

                float pathCost = AStarCost(heightMap, walkableMap, start, end, slopeCostFactor);
                if (pathCost < 0) continue;

                float euclidean = Heuristic(start, end);
                if (euclidean < 1e-3f) continue;

                float stretch = pathCost / euclidean;
                totalStretch += stretch;
                validPaths++;
            }

            return validPaths > 0 ? totalStretch / validPaths : 0f;
        }


    }
}