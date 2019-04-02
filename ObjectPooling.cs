using System;
using System.Collections.Generic;
using System.Linq;

namespace MarittoTools
{
    public class ObjectPooling<AnyObject>
    {
        private List<AnyObject> Unused { get; set; }
        private List<AnyObject> BeingUsed { get; set; }
        private readonly Func<AnyObject> GenerateMore;
        private readonly Action<AnyObject> HideIt;
        private readonly Action<AnyObject> ShowIt;

        /// <summary>
        /// Object pool with the default 16 slots in the list.
        /// </summary>
        /// <param name="HowToGenerateMore">Method to generate more of given object</param>
        /// <param name="HowToHideIt">Method to hide of the given object</param>
        /// <param name="HowToShowIt">Method to show/enable the given object</param>
        public ObjectPooling(Func<AnyObject> HowToGenerateMore, Action<AnyObject> HowToHideIt, Action<AnyObject> HowToShowIt)
        {
            GenerateMore = HowToGenerateMore;
            HideIt = HowToHideIt;
            ShowIt = HowToShowIt;
            Unused = new List<AnyObject>();
            BeingUsed = new List<AnyObject>();
        }

        /// <summary>
        /// Object pool with as many objects created by default as assigned.
        /// </summary>
        /// <param name="HowToGenerateMore">Method to generate more of given object</param>
        /// <param name="HowToHideIt">Method to hide of the given object</param>
        /// <param name="HowToShowIt">Method to show/enable the given object</param>
        /// <param name="PoolHowMany">How many objects to have in the background from the start</param>
        public ObjectPooling(Func<AnyObject> HowToGenerateMore, Action<AnyObject> HowToHideIt, Action<AnyObject> HowToShowIt , int PoolHowMany)
        {
            GenerateMore = HowToGenerateMore;
            HideIt = HowToHideIt;
            ShowIt = HowToShowIt;
            Unused = new List<AnyObject>(PoolHowMany);
            for (int i = 0; i < PoolHowMany; i++)
            {
                AnyObject InstantiatedObject = GenerateMore();
                HideIt(InstantiatedObject);
                Unused.Add(InstantiatedObject);
            }
            BeingUsed = new List<AnyObject>();
        }

        /// <summary>
        /// Retrieves object from list. Creates one if empty.
        /// </summary>
        /// <returns>Object from unused list</returns>
        public AnyObject InstantiatePoolObject()
        {
            AnyObject returnObject;
            if(Unused.Any())
            {
                returnObject = Unused.First();
                Unused.Remove(returnObject);
                BeingUsed.Add(returnObject);
                ShowIt(returnObject);
                return returnObject;
            }

            returnObject = GenerateMore();
            BeingUsed.Add(returnObject);
            return returnObject;
        }
        
        /// <summary>
        /// Disables and Stores the object
        /// </summary>
        /// <param name="ObjectToSaveInPool">The object to disable and store</param>
        public void DestroyPoolObject(AnyObject ObjectToSaveInPool)
        {
            HideIt(ObjectToSaveInPool);
            BeingUsed.Remove(ObjectToSaveInPool);
            Unused.Add(ObjectToSaveInPool);
        }

        /// <summary>
        /// Retrives entire active object list.
        /// </summary>
        /// <returns>Active object list</returns>
        public List<AnyObject> GetAllObjectsCurrentlyActive()
        {
            return BeingUsed;
        }

        /// <summary>
        /// Retrives count  of active objects.
        /// </summary>
        /// <returns>Count of active objects</returns>
        public int CountCurrentlyActive()
        {
            return BeingUsed.Count;
        }

        /// <summary>
        /// Retrieves the count of available on demand objects
        /// </summary>
        /// <returns>Count of on demand objects.</returns>
        public int CountStored()
        {
            return Unused.Count;
        }

    }
}
