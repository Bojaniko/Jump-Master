using UnityEngine;

namespace Studio28.Probability
{
    /// <summary>
    /// A Linear congruential generator.
    /// </summary>
    public class Seeded
    {
        private const int a = 1664525;
        private const int c = 1013904223;
        private readonly int m;

        private readonly int _initialSeed;
        private int _currentSeed;

        private readonly int _target;

        /// <summary>
        /// Generates a random number target and defines an initial seed
        /// for the random number generation.
        /// </summary>
        /// <param name="initial_seed">The initial seed to start from.</param>
        public Seeded(int initial_seed)
        {
            m = (int)Mathf.Pow(2, 32);
            _initialSeed = initial_seed;
            _currentSeed = _initialSeed;
            _target = Random.Range(0, 101);
        }

        /// <summary>
        /// The outcome of a seeded probability.
        /// </summary>
        /// <returns>true if the current random number generated by the seed
        /// equals a target random number initialized in the constructors.</returns>
        public bool Outcome()
        {
            _currentSeed = ((a * _currentSeed + c) % m) / m;
            int number = (int)Mathf.Floor(_currentSeed * 100f);
            return number == _target;
        }

        /// <summary>
        /// Reset the random number generator to the initial seed.
        /// </summary>
        public void Reset()
        {
            _currentSeed = _initialSeed;
        }
    }
}