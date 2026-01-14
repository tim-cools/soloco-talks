using System;

namespace Soloco.Talks.PolyglotPersistence.Infrastructure
{
    // see https://github.com/tim-cools/PsychedelicExperience-server for use
    // use of a BusinessException is a real project
    public class BusinessException : Exception
    {
        public BusinessException(string message) : base(message)
        {
        }
    }
}
