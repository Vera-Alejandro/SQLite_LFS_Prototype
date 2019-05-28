using System;
using System.Configuration;
using System.Runtime.InteropServices;

namespace Interstates.Control.Database
{
    /// <summary>
    /// A collection of DatabaseProfile objects.
    /// </summary>
    [ComVisible(false)]
    public class DatabaseProfiles : ConfigurationElementCollection
    {
        /// <summary>
        /// Initializes a new collection of DatabaseProfiles.
        /// </summary>
        public DatabaseProfiles()
            : base(StringComparer.InvariantCultureIgnoreCase)
        {
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new DatabaseProfile();
        }


        protected override ConfigurationElement CreateNewElement(string elementName)
        {
            return new DatabaseProfile(elementName);
        }

        protected override Object GetElementKey(ConfigurationElement element)
        {
            return ((DatabaseProfile)element).Name;
        }

        public new string AddElementName
        {
            get{ return base.AddElementName; }
            set{ base.AddElementName = value; }

        }

        public new string ClearElementName
        {
            get{ return base.ClearElementName; }
            set{ base.AddElementName = value; }

        }

        public new string RemoveElementName
        {
            get{ return base.RemoveElementName; }
        }

        public new int Count
        {
            get { return base.Count; }
        }

        public DatabaseProfile this[int index]
        {
            get
            {
                return (DatabaseProfile)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        new public DatabaseProfile this[string name]
        {
            get
            {
                return (DatabaseProfile)base.BaseGet(name);
            }
        }

        public int IndexOf(DatabaseProfile profile)
        {
            return BaseIndexOf(profile);
        }

        public void Add(DatabaseProfile profile)
        {
            BaseAdd(profile);
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        public void Remove(DatabaseProfile profile)
        {
            if (BaseIndexOf(profile) >= 0)
                BaseRemove(profile.Name);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }

        public void Clear()
        {
            BaseClear();
        }

        public DatabaseProfiles Copy()
        {
            DatabaseProfiles copyProfiles = new DatabaseProfiles();

            // Make sure the collection is cleared out
            copyProfiles.Clear();
            foreach (DatabaseProfile profile in this)
            {
                copyProfiles.Add(profile.Copy());
            }

            return copyProfiles;
        }
    }
}
