/* Copyright (c) 2018-2023 Nuno Fachada and contributors
 * Distributed under the MIT License (See accompanying file LICENSE or copy
 * at http://opensource.org/licenses/MIT) */

using System;
using System.Linq;
using System.Collections.Generic;

namespace LibGameAI.NaiveBayes
{
    public class NaiveBayesClassifier
    {

        // Number of examples with given label
        private readonly IDictionary<string, int> labelsCount;

        // Number of times each attribute has a given value for each label
        private readonly IDictionary<string, IDictionary<Attrib, AttribCount>>
            attribValueCounts;

        // Constructor, accepts a set of labels and a set of attributes
        public NaiveBayesClassifier(
            IEnumerable<string> labels, IEnumerable<Attrib> attributes)
        {
            // Initilize dictionary containing number of examples with given
            // label
            labelsCount = new Dictionary<string, int>();

            // Initialize dictionary containing number of times each attribute
            // has a given value for each label
            attribValueCounts =
                new Dictionary<string, IDictionary<Attrib, AttribCount>>();

            // Go through each label
            foreach (string label in labels)
            {
                // For the current label, create a dictionary which associates
                // attributes with counts per attribute value
                IDictionary<Attrib, AttribCount> attribCountDict =
                    new Dictionary<Attrib, AttribCount>();

                // Go through each attribute
                foreach (Attrib attrib in attributes)
                {
                    // Initialize attribute value count for current attribute
                    attribCountDict[attrib] = new AttribCount(attrib);
                }

                // Associate the current label with the created attribute-value
                // counts dictionary
                attribValueCounts[label] = attribCountDict;

                // Set total counts for current label to zero
                labelsCount[label] = 0;
            }
        }

        // Update our naive bayes classifier with another observation
        public void Update(
            string label, IDictionary<Attrib, string> attribValues)
        {
            // Update total counts for the given label
            labelsCount[label]++;

            // Go through each attribute
            foreach (Attrib attribute in attribValues.Keys)
            {
                // Get value for current attribute
                string value = attribValues[attribute];

                // Increment count for the current value of the current
                // attribute for the given label
                attribValueCounts[label][attribute].AddCount(value, 1);
            }
        }

        // Make a prediction for a label given a set of attribute-value pairs
        public string Predict(IDictionary<Attrib, string> attribValues)
        {
            // Highest probability so far
            double bestP = 0;
            // Label with highest probability
            string bestLabel = "";
            // Total counts
            int totalCounts = labelsCount.Values.Sum();

            // If there are no observations yet, just return a random label
            if (totalCounts == 0) return labelsCount.Keys
                .ToArray()[DateTime.Now.Millisecond % labelsCount.Keys.Count];

            // Search for label with highest probability
            foreach (string label in labelsCount.Keys)
            {
                // Get total count for current label
                int labelCount = labelsCount[label];

                // Get naive probability for the current label
                double p = NaiveProbabilities(
                    attribValues,
                    attribValueCounts[label],
                    labelCount,
                    totalCounts);

                // Is this the best (most likely) label so far?
                if (p > bestP)
                {
                    // If so, keep it
                    bestP = p;
                    bestLabel = label;
                }
            }

            // Return label with highest probabiliy
            return bestLabel;
        }

        // Get the naive probability for a label given a set of attribute-value
        // pairs
        private double NaiveProbabilities(
            IDictionary<Attrib, string> attribValues,
            IDictionary<Attrib, AttribCount> counts,
            double labelCount,
            double totalCounts)
        {
            // Compute the prior
            double prior = labelCount / totalCounts;

            // Naive assumption of conditional independence
            double p = 1.0;

            // Go through each attribute
            foreach (Attrib attribute in attribValues.Keys)
            {
                // Update the probability
                p = p * counts[attribute].GetCount(attribValues[attribute])
                    / labelCount;
            }

            // Return the final probability
            return prior * p;
        }
    }
}
