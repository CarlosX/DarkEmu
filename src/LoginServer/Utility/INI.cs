using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace LoginServer
{
    public partial class Systems
    {
        public enum ProfileChangeType
        {
            /// <summary> 
            ///   The change refers to the <see cref="Profile.Name" /> property. </summary>		
            /// <remarks> 
            ///   <see cref="ProfileChangedArgs.Value" /> will contain the new name. </remarks>
            Name,

            /// <summary> 
            ///   The change refers to the <see cref="Profile.ReadOnly" /> property. </summary>		
            /// <remarks> 
            ///   <see cref="ProfileChangedArgs.Value" /> will be true. </remarks>
            ReadOnly,

            /// <summary> 
            ///   The change refers to the <see cref="Profile.SetValue" /> method. </summary>		
            /// <remarks> 
            ///   <see cref="ProfileChangedArgs.Section" />,  <see cref="ProfileChangedArgs.Entry" />, 
            ///   and <see cref="ProfileChangedArgs.Value" /> will be set to the same values passed 
            ///   to the SetValue method. </remarks>
            SetValue,

            /// <summary> 
            ///   The change refers to the <see cref="Profile.RemoveEntry" /> method. </summary>		
            /// <remarks> 
            ///   <see cref="ProfileChangedArgs.Section" /> and <see cref="ProfileChangedArgs.Entry" /> 
            ///   will be set to the same values passed to the RemoveEntry method. </remarks>
            RemoveEntry,

            /// <summary> 
            ///   The change refers to the <see cref="Profile.RemoveSection" /> method. </summary>		
            /// <remarks> 
            ///   <see cref="ProfileChangedArgs.Section" /> will contain the name of the section passed to the RemoveSection method. </remarks>
            RemoveSection,

            /// <summary> 
            ///   The change refers to method or property specific to the Profile class. </summary>		
            /// <remarks> 
            ///   <see cref="ProfileChangedArgs.Entry" /> will contain the name of the  method or property.
            ///   <see cref="ProfileChangedArgs.Value" /> will contain the new value. </remarks>
            Other
        }

        /// <summary>
        ///   EventArgs class to be passed as the second parameter of a <see cref="Profile.Changed" /> event handler. </summary>
        /// <remarks>
        ///   This class provides all the information relevant to the change made to the Profile.
        ///   It is also used as a convenient base class for the ProfileChangingArgs class which is passed 
        ///   as the second parameter of the <see cref="Profile.Changing" /> event handler. </remarks>
        /// <seealso cref="ProfileChangingArgs" />
        public class ProfileChangedArgs : EventArgs
        {
            // Fields
            private readonly ProfileChangeType m_changeType;
            private readonly string m_section;
            private readonly string m_entry;
            private readonly object m_value;

            /// <summary>
            ///   Initializes a new instance of the ProfileChangedArgs class by initializing all of its properties. </summary>
            /// <param name="changeType">
            ///   The type of change made to the profile. </param>
            /// <param name="section">
            ///   The name of the section involved in the change or null. </param>
            /// <param name="entry">
            ///   The name of the entry involved in the change, or if changeType is set to Other, the name of the method/property that was changed. </param>
            /// <param name="value">
            ///   The new value for the entry or method/property, based on the value of changeType. </param>
            /// <seealso cref="ProfileChangeType" />
            public ProfileChangedArgs(ProfileChangeType changeType, string section, string entry, object value)
            {
                m_changeType = changeType;
                m_section = section;
                m_entry = entry;
                m_value = value;
            }

            /// <summary>
            ///   Gets the type of change that raised the event. </summary>
            public ProfileChangeType ChangeType
            {
                get
                {
                    return m_changeType;
                }
            }

            /// <summary>
            ///   Gets the name of the section involved in the change, or null if not applicable. </summary>
            public string Section
            {
                get
                {
                    return m_section;
                }
            }

            /// <summary>
            ///   Gets the name of the entry involved in the change, or null if not applicable. </summary>
            /// <remarks> 
            ///   If <see cref="ChangeType" /> is set to Other, this property holds the name of the 
            ///   method/property that was changed. </remarks>
            public string Entry
            {
                get
                {
                    return m_entry;
                }
            }

            /// <summary>
            ///   Gets the new value for the entry or method/property, based on the value of <see cref="ChangeType" />. </summary>
            public object Value
            {
                get
                {
                    return m_value;
                }
            }
        }

        /// <summary>
        ///   EventArgs class to be passed as the second parameter of a <see cref="Profile.Changing" /> event handler. </summary>
        /// <remarks>
        ///   This class provides all the information relevant to the change about to be made to the Profile.
        ///   Besides the properties of ProfileChangedArgs, it adds the Cancel property which allows the 
        ///   event handler to prevent the change from happening. </remarks>
        /// <seealso cref="ProfileChangedArgs" />
        public class ProfileChangingArgs : ProfileChangedArgs
        {
            private bool m_cancel;

            /// <summary>
            ///   Initializes a new instance of the ProfileChangingArgs class by initializing all of its properties. </summary>
            /// <param name="changeType">
            ///   The type of change to be made to the profile. </param>
            /// <param name="section">
            ///   The name of the section involved in the change or null. </param>
            /// <param name="entry">
            ///   The name of the entry involved in the change, or if changeType is set to Other, the name of the method/property that was changed. </param>
            /// <param name="value">
            ///   The new value for the entry or method/property, based on the value of changeType. </param>
            /// <seealso cref="ProfileChangeType" />
            public ProfileChangingArgs(ProfileChangeType changeType, string section, string entry, object value) :
                base(changeType, section, entry, value)
            {
            }

            /// <summary>
            ///   Gets or sets whether the change about to the made should be canceled or not. </summary>
            /// <remarks> 
            ///   By default this property is set to false, meaning that the change is allowed.  </remarks>
            public bool Cancel
            {
                get
                {
                    return m_cancel;
                }
                set
                {
                    m_cancel = value;
                }
            }
        }

        /// <summary>
        ///   Definition of the <see cref="Profile.Changing" /> event handler. </summary>
        /// <remarks>
        ///   This definition complies with the .NET Framework's standard for event handlers.
        ///   The sender is always set to the Profile object that raised the event. </remarks>
        public delegate void ProfileChangingHandler(object sender, ProfileChangingArgs e);

        /// <summary>
        ///   Definition of the <see cref="Profile.Changed" /> event handler. </summary>
        /// <remarks>
        ///   This definition complies with the .NET Framework's standard for event handlers.
        ///   The sender is always set to the Profile object that raised the event. </remarks>
        public delegate void ProfileChangedHandler(object sender, ProfileChangedArgs e);

        public interface IReadOnlyProfile : ICloneable
        {
            /// <summary>
            ///   Gets the name associated with the profile. </summary>
            /// <remarks>
            ///   This should be the name of the file where the data is stored, or something equivalent. </remarks>
            string Name
            {
                get;
            }

            /// <summary>
            ///   Retrieves the value of an entry inside a section. </summary>
            /// <param name="section">
            ///   The name of the section that holds the entry with the value. </param>
            /// <param name="entry">
            ///   The name of the entry where the value is stored. </param>
            /// <returns>
            ///   The return value should be the entry's value, or null if the entry does not exist. </returns>
            /// <seealso cref="HasEntry" />
            object GetValue(string section, string entry);

            /// <summary>
            ///   Retrieves the value of an entry inside a section, or a default value if the entry does not exist. </summary>
            /// <param name="section">
            ///   The name of the section that holds the entry with the value. </param>
            /// <param name="entry">
            ///   The name of the entry where the value is stored. </param>
            /// <param name="defaultValue">
            ///   The value to return if the entry (or section) does not exist. </param>
            /// <returns>
            ///   The return value should be the entry's value converted to a string, or the given default value if the entry does not exist. </returns>
            /// <seealso cref="HasEntry" />
            string GetValue(string section, string entry, string defaultValue);

            /// <summary>
            ///   Retrieves the value of an entry inside a section, or a default value if the entry does not exist. </summary>
            /// <param name="section">
            ///   The name of the section that holds the entry with the value. </param>
            /// <param name="entry">
            ///   The name of the entry where the value is stored. </param>
            /// <param name="defaultValue">
            ///   The value to return if the entry (or section) does not exist. </param>
            /// <returns>
            ///   The return value should be the entry's value converted to an integer.  If the value
            ///   cannot be converted, the return value should be 0.  If the entry does not exist, the
            ///   given default value should be returned. </returns>
            /// <seealso cref="HasEntry" />
            int GetValue(string section, string entry, int defaultValue);

            /// <summary>
            ///   Retrieves the value of an entry inside a section, or a default value if the entry does not exist. </summary>
            /// <param name="section">
            ///   The name of the section that holds the entry with the value. </param>
            /// <param name="entry">
            ///   The name of the entry where the value is stored. </param>
            /// <param name="defaultValue">
            ///   The value to return if the entry (or section) does not exist. </param>
            /// <returns>
            ///   The return value should be the entry's value converted to a double.  If the value
            ///   cannot be converted, the return value should be 0.  If the entry does not exist, the
            ///   given default value should be returned. </returns>
            /// <seealso cref="HasEntry" />
            double GetValue(string section, string entry, double defaultValue);

            /// <summary>
            ///   Retrieves the value of an entry inside a section, or a default value if the entry does not exist. </summary>
            /// <param name="section">
            ///   The name of the section that holds the entry with the value. </param>
            /// <param name="entry">
            ///   The name of the entry where the value is stored. </param>
            /// <param name="defaultValue">
            ///   The value to return if the entry (or section) does not exist. </param>
            /// <returns>
            ///   The return value should be the entry's value converted to a bool.  If the value
            ///   cannot be converted, the return value should be <c>false</c>.  If the entry does not exist, the
            ///   given default value should be returned. </returns>
            /// <remarks>
            ///   Note: Boolean values are stored as "True" or "False". </remarks>
            /// <seealso cref="HasEntry" />
            bool GetValue(string section, string entry, bool defaultValue);

            /// <summary>
            ///   Determines if an entry exists inside a section. </summary>
            /// <param name="section">
            ///   The name of the section that holds the entry. </param>
            /// <param name="entry">
            ///   The name of the entry to be checked for existence. </param>
            /// <returns>
            ///   If the entry exists inside the section, the return value should be true; otherwise false. </returns>
            /// <seealso cref="HasSection" />
            /// <seealso cref="GetEntryNames" />
            bool HasEntry(string section, string entry);

            /// <summary>
            ///   Determines if a section exists. </summary>
            /// <param name="section">
            ///   The name of the section to be checked for existence. </param>
            /// <returns>
            ///   If the section exists, the return value should be true; otherwise false. </returns>
            /// <seealso cref="HasEntry" />
            /// <seealso cref="GetSectionNames" />
            bool HasSection(string section);

            /// <summary>
            ///   Retrieves the names of all the entries inside a section. </summary>
            /// <param name="section">
            ///   The name of the section holding the entries. </param>
            /// <returns>
            ///   If the section exists, the return value should be an array with the names of its entries; 
            ///   otherwise it should be null. </returns>
            /// <seealso cref="HasEntry" />
            /// <seealso cref="GetSectionNames" />
            string[] GetEntryNames(string section);

            /// <summary>
            ///   Retrieves the names of all the sections. </summary>
            /// <returns>
            ///   The return value should be an array with the names of all the sections. </returns>
            /// <seealso cref="HasSection" />
            /// <seealso cref="GetEntryNames" />
            string[] GetSectionNames();

            /// <summary>
            ///   Retrieves a DataSet object containing every section, entry, and value in the profile. </summary>
            /// <returns>
            ///   If the profile exists, the return value should be a DataSet object representing the profile; otherwise it's null. </returns>
            /// <remarks>
            ///   The returned DataSet should be named using the <see cref="Name" /> property.  
            ///   It should contain one table for each section, and each entry should be represented by a column inside the table.
            ///   Each table should contain only one row where the values will be stored corresponding to each column (entry). 
            ///   <para>
            ///   This method serves as a convenient way to extract the profile's data to this generic medium known as the DataSet.  
            ///   This allows it to be moved to many different places, including a different type of profile object 
            ///   (eg., INI to XML conversion). </para>
            /// </remarks>
            System.Data.DataSet GetDataSet();
        }



        /// <summary>
        ///   Interface implemented by all profile classes in this namespace.
        ///   It represents a normal profile. </summary>
        /// <remarks>
        ///   This interface takes the members of IReadOnlyProfile (its base interface) and adds
        ///   to it the rest of the members, which allow modifications to the profile.  
        ///   Altogether, this represents a complete profile object. </remarks>
        /// <seealso cref="IReadOnlyProfile" />
        /// <seealso cref="Profile" />

        public interface IProfile : IReadOnlyProfile
        {
            /// <summary>
            ///   Gets or sets the name associated with the profile. </summary>
            /// <remarks>
            ///   This should be the name of the file where the data is stored, or something equivalent.
            ///   When setting this property, the <see cref="ReadOnly" /> property should be checked and if true, an InvalidOperationException should be raised.
            ///   The <see cref="Changing" /> and <see cref="Changed" /> events should be raised before and after this property is changed. </remarks>
            /// <seealso cref="DefaultName" />
            new string Name
            {
                get;
                set;
            }

            /// <summary>
            ///   Gets the name associated with the profile by default. </summary>
            /// <remarks>
            ///   This is used to set the default Name of the profile and it is typically based on 
            ///   the name of the executable plus some extension. </remarks>
            /// <seealso cref="Name" />
            string DefaultName
            {
                get;
            }

            /// <summary>
            ///   Gets or sets whether the profile is read-only or not. </summary>
            /// <remarks>
            ///   A read-only profile should not allow any operations that alter sections,
            ///   entries, or values, such as <see cref="SetValue" /> or <see cref="RemoveEntry" />.  
            ///   Once a profile has been marked read-only, it should be allowed to go back; 
            ///   attempting to do so should cause an InvalidOperationException to be raised.
            ///   The <see cref="Changing" /> and <see cref="Changed" /> events should be raised before 
            ///   and after this property is changed. </remarks>
            /// <seealso cref="CloneReadOnly" />
            /// <seealso cref="IReadOnlyProfile" />
            bool ReadOnly
            {
                get;
                set;
            }

            /// <summary>
            ///   Sets the value for an entry inside a section. </summary>
            /// <param name="section">
            ///   The name of the section that holds the entry. </param>
            /// <param name="entry">
            ///   The name of the entry where the value will be set. </param>
            /// <param name="value">
            ///   The value to set. If it's null, the entry should be removed. </param>
            /// <remarks>
            ///   This method should check the <see cref="ReadOnly" /> property and throw an InvalidOperationException if it's true.
            ///   It should also raise the <see cref="Changing" /> and <see cref="Changed" /> events before and after the value is set. </remarks>
            /// <seealso cref="IReadOnlyProfile.GetValue" />
            void SetValue(string section, string entry, object value);

            /// <summary>
            ///   Removes an entry from a section. </summary>
            /// <param name="section">
            ///   The name of the section that holds the entry. </param>
            /// <param name="entry">
            ///   The name of the entry to remove. </param>
            /// <remarks>
            ///   This method should check the <see cref="ReadOnly" /> property and throw an InvalidOperationException if it's true.
            ///   It should also raise the <see cref="Changing" /> and <see cref="Changed" /> events before and after the entry is removed. </remarks>
            /// <seealso cref="RemoveSection" />
            void RemoveEntry(string section, string entry);

            /// <summary>
            ///   Removes a section. </summary>
            /// <param name="section">
            ///   The name of the section to remove. </param>
            /// <remarks>
            ///   This method should check the <see cref="ReadOnly" /> property and throw an InvalidOperationException if it's true.
            ///   It should also raise the <see cref="Changing" /> and <see cref="Changed" /> events before and after the section is removed. </remarks>
            /// <seealso cref="RemoveEntry" />
            void RemoveSection(string section);

            /// <summary>
            ///   Writes the data of every table from a DataSet into this profile. </summary>
            /// <param name="ds">
            ///   The DataSet object containing the data to be set. </param>
            /// <remarks>
            ///   Each table in the DataSet should be used to represent a section of the profile.  
            ///   Each column of each table should represent an entry.  And for each column, the corresponding value
            ///   of the first row is the value that should be passed to <see cref="SetValue" />.  
            ///   <para>
            ///   This method serves as a convenient way to take any data inside a generic DataSet and 
            ///   write it to any of the available profiles. </para></remarks>
            /// <seealso cref="IReadOnlyProfile.GetDataSet" />
            void SetDataSet(DataSet ds);

            /// <summary>
            ///   Creates a copy of itself and makes it read-only. </summary>
            /// <returns>
            ///   The return value should be a copy of itself as an IReadOnlyProfile object. </returns>
            /// <remarks>
            ///   This method is meant as a convenient way to pass a read-only copy of the profile to methods 
            ///   that are not allowed to modify it. </remarks>
            /// <seealso cref="ReadOnly" />
            IReadOnlyProfile CloneReadOnly();

            /// <summary>
            ///   Event that should be raised just before the profile is to be changed to allow the change to be canceled. </summary>
            /// <seealso cref="Changed" />
            event ProfileChangingHandler Changing;

            /// <summary>
            ///   Event that should be raised right after the profile has been changed. </summary>
            /// <seealso cref="Changing" />
            event ProfileChangedHandler Changed;
        }


        public abstract class Profile : IProfile
        {
            // Fields
            private string m_name;
            private bool m_readOnly;

            /// <summary>
            ///   Event used to notify that the profile is about to be changed. </summary>
            /// <seealso cref="Changed" />
            public event ProfileChangingHandler Changing;

            /// <summary>
            ///   Event used to notify that the profile has been changed. </summary>
            /// <seealso cref="Changing" />
            public event ProfileChangedHandler Changed;

            /// <summary>
            ///   Initializes a new instance of the Profile class by setting the <see cref="Name" /> to <see cref="DefaultName" />. </summary>
            protected Profile()
            {
                m_name = DefaultName;
            }

            /// <summary>
            ///   Initializes a new instance of the Profile class by setting the <see cref="Name" /> to a value. </summary>
            /// <param name="name">
            ///   The name to initialize the <see cref="Name" /> property with. </param>
            protected Profile(string name)
            {
                m_name = name;
            }

            /// <summary>
            ///   Initializes a new instance of the Profile class based on another Profile object. </summary>
            /// <param name="profile">
            ///   The Profile object whose properties and events are used to initialize the object being constructed. </param>
            protected Profile(Profile profil)
            {
                m_name = profil.m_name;
                m_readOnly = profil.m_readOnly;
                Changing = profil.Changing;
                Changed = profil.Changed;
            }

            /// <summary>
            ///   Gets or sets the name associated with the profile. </summary>
            /// <exception cref="NullReferenceException">
            ///   Setting this property to null. </exception>
            /// <exception cref="InvalidOperationException">
            ///   Setting this property if ReadOnly is true. </exception>
            /// <remarks>
            ///   This is usually the name of the file where the data is stored. 
            ///   The <see cref="Changing" /> event is raised before changing this property.  
            ///   If its <see cref="ProfileChangingArgs.Cancel" /> property is set to true, this property 
            ///   returns immediately without being changed.  After the property is changed, 
            ///   the <see cref="Changed" /> event is raised. </remarks>
            /// <seealso cref="DefaultName" />
            public string Name
            {
                get
                {
                    return m_name;
                }
                set
                {
                    VerifyNotReadOnly();
                    if (m_name == value.Trim())
                        return;

                    if (!RaiseChangeEvent(true, ProfileChangeType.Name, null, null, value))
                        return;

                    m_name = value.Trim();
                    RaiseChangeEvent(false, ProfileChangeType.Name, null, null, value);
                }
            }

            /// <summary>
            ///   Gets or sets whether the profile is read-only or not. </summary>
            /// <exception cref="InvalidOperationException">
            ///   Setting this property if it's already true. </exception>
            /// <remarks>
            ///   A read-only profile does not allow any operations that alter sections,
            ///   entries, or values, such as <see cref="SetValue" /> or <see cref="RemoveEntry" />.  
            ///   Once a profile has been marked read-only, it may no longer go back; 
            ///   attempting to do so causes an InvalidOperationException to be raised.
            ///   The <see cref="Changing" /> event is raised before changing this property.  
            ///   If its <see cref="ProfileChangingArgs.Cancel" /> property is set to true, this property 
            ///   returns immediately without being changed.  After the property is changed, 
            ///   the <see cref="Changed" /> event is raised. </remarks>
            /// <seealso cref="CloneReadOnly" />
            /// <seealso cref="IReadOnlyProfile" />
            public bool ReadOnly
            {
                get
                {
                    return m_readOnly;
                }

                set
                {
                    VerifyNotReadOnly();
                    if (m_readOnly == value)
                        return;

                    if (!RaiseChangeEvent(true, ProfileChangeType.ReadOnly, null, null, value))
                        return;

                    m_readOnly = value;
                    RaiseChangeEvent(false, ProfileChangeType.ReadOnly, null, null, value);
                }
            }

            /// <summary>
            ///   Gets the name associated with the profile by default. </summary>
            /// <remarks>
            ///   This property needs to be implemented by derived classes.  
            ///   See <see cref="IProfile.DefaultName">IProfile.DefaultName</see> for additional remarks. </remarks>
            /// <seealso cref="Name" />
            public abstract string DefaultName
            {
                get;
            }

            /// <summary>
            ///   Retrieves a copy of itself. </summary>
            /// <returns>
            ///   The return value is a copy of itself as an object. </returns>
            /// <remarks>
            ///   This method needs to be implemented by derived classes. </remarks>
            /// <seealso cref="CloneReadOnly" />
            public abstract object Clone();

            /// <summary>
            ///   Sets the value for an entry inside a section. </summary>
            /// <param name="section">
            ///   The name of the section that holds the entry. </param>
            /// <param name="entry">
            ///   The name of the entry where the value will be set. </param>
            /// <param name="value">
            ///   The value to set. If it's null, the entry should be removed. </param>
            /// <exception cref="InvalidOperationException">
            ///   <see cref="Profile.ReadOnly" /> is true or
            ///   <see cref="Profile.Name" /> is null or empty. </exception>
            /// <exception cref="ArgumentNullException">
            ///   Either section or entry is null. </exception>
            /// <remarks>
            ///   This method needs to be implemented by derived classes.  Check the 
            ///   documentation to see what other exceptions derived versions may raise.
            ///   See <see cref="IProfile.SetValue">IProfile.SetValue</see> for additional remarks. </remarks>
            /// <seealso cref="GetValue" />
            public abstract void SetValue(string section, string entry, object value);

            /// <summary>
            ///   Retrieves the value of an entry inside a section. </summary>
            /// <param name="section">
            ///   The name of the section that holds the entry with the value. </param>
            /// <param name="entry">
            ///   The name of the entry where the value is stored. </param>
            /// <returns>
            ///   The return value is the entry's value, or null if the entry does not exist. </returns>
            /// <exception cref="InvalidOperationException">
            ///	  <see cref="Profile.Name" /> is null or empty. </exception>
            /// <exception cref="ArgumentNullException">
            ///   Either section or entry is null. </exception>
            /// <remarks>
            ///   This method needs to be implemented by derived classes.  Check the 
            ///   documentation to see what other exceptions derived versions may raise. </remarks>
            /// <seealso cref="SetValue" />
            /// <seealso cref="HasEntry" />
            public abstract object GetValue(string section, string entry);

            /// <summary>
            ///   Retrieves the string value of an entry inside a section, or a default value if the entry does not exist. </summary>
            /// <param name="section">
            ///   The name of the section that holds the entry with the value. </param>
            /// <param name="entry">
            ///   The name of the entry where the value is stored. </param>
            /// <param name="defaultValue">
            ///   The value to return if the entry (or section) does not exist. </param>
            /// <returns>
            ///   The return value is the entry's value converted to a string, or the given default value if the entry does not exist. </returns>
            /// <exception cref="InvalidOperationException">
            ///	  <see cref="Profile.Name" /> is null or empty. </exception>
            /// <exception cref="ArgumentNullException">
            ///   Either section or entry is null. </exception>
            /// <remarks>
            ///   This method calls <c>GetValue(section, entry)</c> of the derived class, so check its 
            ///   documentation to see what other exceptions may be raised. </remarks>
            /// <seealso cref="SetValue" />
            /// <seealso cref="HasEntry" />
            public virtual string GetValue(string section, string entry, string defaultValue)
            {
                object value = GetValue(section, entry);
                return (value == null ? defaultValue : value.ToString());
            }

            /// <summary>
            ///   Retrieves the integer value of an entry inside a section, or a default value if the entry does not exist. </summary>
            /// <param name="section">
            ///   The name of the section that holds the entry with the value. </param>
            /// <param name="entry">
            ///   The name of the entry where the value is stored. </param>
            /// <param name="defaultValue">
            ///   The value to return if the entry (or section) does not exist. </param>
            /// <returns>
            ///   The return value is the entry's value converted to an integer.  If the value
            ///   cannot be converted, the return value is 0.  If the entry does not exist, the
            ///   given default value is returned. </returns>
            /// <exception cref="InvalidOperationException">
            ///	  <see cref="Profile.Name" /> is null or empty. </exception>
            /// <exception cref="ArgumentNullException">
            ///   Either section or entry is null. </exception>
            /// <remarks>
            ///   This method calls <c>GetValue(section, entry)</c> of the derived class, so check its 
            ///   documentation to see what other exceptions may be raised. </remarks>
            /// <seealso cref="SetValue" />
            /// <seealso cref="HasEntry" />
            public virtual int GetValue(string section, string entry, int defaultValue)
            {
                object value = GetValue(section, entry);
                if (value == null)
                    return defaultValue;

                try
                {
                    return Convert.ToInt32(value);
                }
                catch
                {
                    return 0;
                }
            }

            /// <summary>
            ///   Retrieves the double value of an entry inside a section, or a default value if the entry does not exist. </summary>
            /// <param name="section">
            ///   The name of the section that holds the entry with the value. </param>
            /// <param name="entry">
            ///   The name of the entry where the value is stored. </param>
            /// <param name="defaultValue">
            ///   The value to return if the entry (or section) does not exist. </param>
            /// <returns>
            ///   The return value is the entry's value converted to a double.  If the value
            ///   cannot be converted, the return value is 0.  If the entry does not exist, the
            ///   given default value is returned. </returns>
            /// <exception cref="InvalidOperationException">
            ///	  <see cref="Profile.Name" /> is null or empty. </exception>
            /// <exception cref="ArgumentNullException">
            ///   Either section or entry is null. </exception>
            /// <remarks>
            ///   This method calls <c>GetValue(section, entry)</c> of the derived class, so check its 
            ///   documentation to see what other exceptions may be raised. </remarks>
            /// <seealso cref="SetValue" />
            /// <seealso cref="HasEntry" />
            public virtual double GetValue(string section, string entry, double defaultValue)
            {
                object value = GetValue(section, entry);
                if (value == null)
                    return defaultValue;

                try
                {
                    return Convert.ToDouble(value);
                }
                catch
                {
                    return 0;
                }
            }

            /// <summary>
            ///   Retrieves the bool value of an entry inside a section, or a default value if the entry does not exist. </summary>
            /// <param name="section">
            ///   The name of the section that holds the entry with the value. </param>
            /// <param name="entry">
            ///   The name of the entry where the value is stored. </param>
            /// <param name="defaultValue">
            ///   The value to return if the entry (or section) does not exist. </param>
            /// <returns>
            ///   The return value is the entry's value converted to a bool.  If the value
            ///   cannot be converted, the return value is <c>false</c>.  If the entry does not exist, the
            ///   given default value is returned. </returns>
            /// <exception cref="InvalidOperationException">
            ///	  <see cref="Profile.Name" /> is null or empty. </exception>
            /// <exception cref="ArgumentNullException">
            ///   Either section or entry is null. </exception>
            /// <remarks>
            ///   Note: Boolean values are stored as "True" or "False". 
            ///   <para>
            ///   This method calls <c>GetValue(section, entry)</c> of the derived class, so check its 
            ///   documentation to see what other exceptions may be raised. </para></remarks>
            /// <seealso cref="SetValue" />
            /// <seealso cref="HasEntry" />
            public virtual bool GetValue(string section, string entry, bool defaultValue)
            {
                object value = GetValue(section, entry);
                if (value == null)
                    return defaultValue;

                try
                {
                    return Convert.ToBoolean(value);
                }
                catch
                {
                    return false;
                }
            }

            /// <summary>
            ///   Determines if an entry exists inside a section. </summary>
            /// <param name="section">
            ///   The name of the section that holds the entry. </param>
            /// <param name="entry">
            ///   The name of the entry to be checked for existence. </param>
            /// <returns>
            ///   If the entry exists inside the section, the return value is true; otherwise false. </returns>
            /// <exception cref="ArgumentNullException">
            ///   section is null. </exception>
            /// <remarks>
            ///   This method calls GetEntryNames of the derived class, so check its 
            ///   documentation to see what other exceptions may be raised. </remarks>
            /// <seealso cref="HasSection" />
            /// <seealso cref="GetEntryNames" />
            public virtual bool HasEntry(string section, string entry)
            {
                string[] entries = GetEntryNames(section);

                if (entries == null)
                    return false;

                VerifyAndAdjustEntry(ref entry);
                return Array.IndexOf(entries, entry) >= 0;
            }

            /// <summary>
            ///   Determines if a section exists. </summary>
            /// <param name="section">
            ///   The name of the section to be checked for existence. </param>
            /// <returns>
            ///   If the section exists, the return value is true; otherwise false. </returns>
            /// <seealso cref="HasEntry" />
            /// <seealso cref="GetSectionNames" />
            public virtual bool HasSection(string section)
            {
                string[] sections = GetSectionNames();

                if (sections == null)
                    return false;

                VerifyAndAdjustSection(ref section);
                return Array.IndexOf(sections, section) >= 0;
            }

            /// <summary>
            ///   Removes an entry from a section. </summary>
            /// <param name="section">
            ///   The name of the section that holds the entry. </param>
            /// <param name="entry">
            ///   The name of the entry to remove. </param>
            /// <exception cref="InvalidOperationException">
            ///   <see cref="Profile.ReadOnly" /> is true. </exception>
            /// <exception cref="ArgumentNullException">
            ///   Either section or entry is null. </exception>
            /// <remarks>
            ///   This method needs to be implemented by derived classes.  Check the 
            ///   documentation to see what other exceptions derived versions may raise.
            ///   See <see cref="IProfile.RemoveEntry">IProfile.RemoveEntry</see> for additional remarks. </remarks>
            /// <seealso cref="RemoveSection" />
            public abstract void RemoveEntry(string section, string entry);

            /// <summary>
            ///   Removes a section. </summary>
            /// <param name="section">
            ///   The name of the section to remove. </param>
            /// <exception cref="InvalidOperationException">
            ///   <see cref="Profile.ReadOnly" /> is true. </exception>
            /// <exception cref="ArgumentNullException">
            ///   section is null. </exception>
            /// <remarks>
            ///   This method needs to be implemented by derived classes.  Check the 
            ///   documentation to see what other exceptions derived versions may raise.
            ///   See <see cref="IProfile.RemoveSection">IProfile.RemoveSection</see> for additional remarks. </remarks>
            /// <seealso cref="RemoveEntry" />
            public abstract void RemoveSection(string section);

            /// <summary>
            ///   Retrieves the names of all the entries inside a section. </summary>
            /// <param name="section">
            ///   The name of the section holding the entries. </param>
            /// <returns>
            ///   If the section exists, the return value should be an array with the names of its entries; 
            ///   otherwise null. </returns>
            /// <exception cref="ArgumentNullException">
            ///   section is null. </exception>
            /// <remarks>
            ///   This method needs to be implemented by derived classes.  Check the 
            ///   documentation to see what other exceptions derived versions may raise. </remarks>
            /// <seealso cref="HasEntry" />
            /// <seealso cref="GetSectionNames" />
            public abstract string[] GetEntryNames(string section);

            /// <summary>
            ///   Retrieves the names of all the sections. </summary>
            /// <returns>
            ///   The return value should be an array with the names of all the sections. </returns>
            /// <remarks>
            ///   This method needs to be implemented by derived classes.  Check the 
            ///   documentation to see what exceptions derived versions may raise. </remarks>
            /// <seealso cref="HasSection" />
            /// <seealso cref="GetEntryNames" />
            public abstract string[] GetSectionNames();

            /// <summary>
            ///   Retrieves a copy of itself and makes it read-only. </summary>
            /// <returns>
            ///   The return value is a copy of itself as a IReadOnlyProfile object. </returns>
            /// <remarks>
            ///   This method serves as a convenient way to pass a read-only copy of the profile to methods 
            ///   that are not allowed to modify it. </remarks>
            /// <seealso cref="ReadOnly" />
            public virtual IReadOnlyProfile CloneReadOnly()
            {
                Profile profile = (Profile)Clone();
                profile.m_readOnly = true;

                return profile;
            }

            /// <summary>
            ///   Retrieves a DataSet object containing every section, entry, and value in the profile. </summary>
            /// <returns>
            ///   If the profile exists, the return value is a DataSet object representing the profile; otherwise it's null. </returns>
            /// <exception cref="InvalidOperationException">
            ///	  <see cref="Profile.Name" /> is null or empty. </exception>
            /// <remarks>
            ///   The returned DataSet will be named using the <see cref="Name" /> property.  
            ///   It will contain one table for each section, and each entry will be represented by a column inside the table.
            ///   Each table will contain only one row where the values will stored corresponding to each column (entry). 
            ///   <para>
            ///   This method serves as a convenient way to extract the profile's data to this generic medium known as the DataSet.  
            ///   This allows it to be moved to many different places, including a different type of profile object 
            ///   (eg., INI to XML conversion). </para>
            ///   <para>
            ///   This method calls GetSectionNames, GetEntryNames, and GetValue of the derived class, so check the 
            ///   documentation to see what other exceptions may be raised. </para></remarks>
            /// <seealso cref="SetDataSet" />
            public virtual DataSet GetDataSet()
            {
                VerifyName();

                string[] sections = GetSectionNames();
                if (sections == null)
                    return null;

                DataSet ds = new DataSet(Name);

                // Add a table for each section
                foreach (string section in sections)
                {
                    DataTable table = ds.Tables.Add(section);

                    // Retrieve the column names and values
                    string[] entries = GetEntryNames(section);
                    DataColumn[] columns = new DataColumn[entries.Length];
                    object[] values = new object[entries.Length];

                    int i = 0;
                    foreach (string entry in entries)
                    {
                        object value = GetValue(section, entry);

                        columns[i] = new DataColumn(entry, value.GetType());
                        values[i++] = value;
                    }

                    // Add the columns and values to the table
                    table.Columns.AddRange(columns);
                    table.Rows.Add(values);
                }

                return ds;
            }

            /// <summary>
            ///   Writes the data of every table from a DataSet into this profile. </summary>
            /// <param name="ds">
            ///   The DataSet object containing the data to be set. </param>
            /// <exception cref="InvalidOperationException">
            ///   <see cref="Profile.ReadOnly" /> is true or
            ///   <see cref="Profile.Name" /> is null or empty. </exception>
            /// <exception cref="ArgumentNullException">
            ///   ds is null. </exception>
            /// <remarks>
            ///   Each table in the DataSet represents a section of the profile.  
            ///   Each column of each table represents an entry.  And for each column, the corresponding value
            ///   of the first row is the value to be passed to <see cref="SetValue" />.  
            ///   Note that only the first row is imported; additional rows are ignored.
            ///   <para>
            ///   This method serves as a convenient way to take any data inside a generic DataSet and 
            ///   write it to any of the available profiles. </para>
            ///   <para>
            ///   This method calls SetValue of the derived class, so check its 
            ///   documentation to see what other exceptions may be raised. </para></remarks>
            /// <seealso cref="GetDataSet" />
            public virtual void SetDataSet(DataSet ds)
            {
                if (ds == null)
                    throw new ArgumentNullException("ds");

                // Create a section for each table
                foreach (DataTable table in ds.Tables)
                {
                    string section = table.TableName;
                    DataRowCollection rows = table.Rows;
                    if (rows.Count == 0)
                        continue;

                    // Loop through each column and add it as entry with value of the first row				
                    foreach (DataColumn column in table.Columns)
                    {
                        string entry = column.ColumnName;
                        object value = rows[0][column];

                        SetValue(section, entry, value);
                    }
                }
            }

            /// <summary>
            ///   Gets the name of the file to be used as the default, without the profile-specific extension. </summary>
            /// <remarks>
            ///   This property is used by file-based Profile implementations 
            ///   when composing the DefaultName.  These implementations take the value returned by this
            ///   property and add their own specific extension (.ini, .xml, .config, etc.).
            ///   <para>
            ///   For Windows applications, this property returns the full path of the executable.  
            ///   For Web applications, this returns the full path of the web.config file without 
            ///   the .config extension.  </para></remarks>
            /// <seealso cref="DefaultName" />
            protected string DefaultNameWithoutExtension
            {
                get
                {
                    try
                    {
                        string file = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
                        return file.Substring(0, file.LastIndexOf('.'));
                    }
                    catch
                    {
                        return "profile";  // if all else fails
                    }
                }
            }

            /// <summary>
            ///   Verifies the given section name is not null and trims it. </summary>
            /// <param name="section">
            ///   The section name to verify and adjust. </param>
            /// <exception cref="ArgumentNullException">
            ///   section is null. </exception>
            /// <remarks>
            ///   This method may be used by derived classes to make sure that a valid
            ///   section name has been passed, and to make any necessary adjustments to it
            ///   before passing it to the corresponding APIs. </remarks>
            /// <seealso cref="VerifyAndAdjustEntry" />
            protected virtual void VerifyAndAdjustSection(ref string section)
            {
                if (section == null)
                    throw new ArgumentNullException("section");

                section = section.Trim();
            }

            /// <summary>
            ///   Verifies the given entry name is not null and trims it. </summary>
            /// <param name="entry">
            ///   The entry name to verify and adjust. </param>
            /// <remarks>
            ///   This method may be used by derived classes to make sure that a valid
            ///   entry name has been passed, and to make any necessary adjustments to it
            ///   before passing it to the corresponding APIs. </remarks>
            /// <exception cref="ArgumentNullException">
            ///   entry is null. </exception>
            /// <seealso cref="VerifyAndAdjustSection" />
            protected virtual void VerifyAndAdjustEntry(ref string entry)
            {
                if (entry == null)
                    throw new ArgumentNullException("entry");

                entry = entry.Trim();
            }

            /// <summary>
            ///   Verifies the Name property is not empty or null. </summary>
            /// <remarks>
            ///   This method may be used by derived classes to make sure that the 
            ///   APIs are working with a valid Name (file name) </remarks>
            /// <exception cref="InvalidOperationException">
            ///   name is empty or null. </exception>
            /// <seealso cref="Name" />
            protected internal virtual void VerifyName()
            {
                if (m_name == null || m_name == "")
                    throw new InvalidOperationException("Operation not allowed because Name property is null or empty.");
            }

            /// <summary>
            ///   Verifies the ReadOnly property is not true. </summary>
            /// <remarks>
            ///   This method may be used by derived classes as a convenient way to 
            ///   validate that modifications to the profile can be made. </remarks>
            /// <exception cref="InvalidOperationException">
            ///   ReadOnly is true. </exception>
            /// <seealso cref="ReadOnly" />
            protected internal virtual void VerifyNotReadOnly()
            {
                if (m_readOnly)
                    throw new InvalidOperationException("Operation not allowed because ReadOnly property is true.");
            }

            /// <summary>
            ///   Raises either the Changing or Changed event. </summary>
            /// <param name="changing">
            ///   If true, the Changing event is raised otherwise it's Changed. </param>
            /// <param name="changeType">
            ///   The type of change being made. </param>
            /// <param name="section">
            ///   The name of the section that was involved in the change or null if not applicable. </param>
            /// <param name="entry">
            ///   The name of the entry that was involved in the change or null if not applicable. 
            ///   If changeType is equal to Other, entry is the name of the property involved in the change.</param>
            /// <param name="value">
            ///   The value that was changed or null if not applicable. </param>
            /// <returns>
            ///   The return value is based on the event raised.  If the Changing event was raised, 
            ///   the return value is the opposite of ProfileChangingArgs.Cancel; otherwise it's true.</returns>
            /// <remarks>
            ///   This method may be used by derived classes as a convenient alternative to calling 
            ///   OnChanging and OnChanged.  For example, a typical call to OnChanging would require
            ///   four lines of code, which this method reduces to two. </remarks>
            /// <seealso cref="Changing" />
            /// <seealso cref="Changed" />
            /// <seealso cref="OnChanging" />
            /// <seealso cref="OnChanged" />
            protected bool RaiseChangeEvent(bool changing, ProfileChangeType changeType, string section, string entry, object value)
            {
                if (changing)
                {
                    // Don't even bother if there are no handlers.
                    if (Changing == null)
                        return true;

                    ProfileChangingArgs e = new ProfileChangingArgs(changeType, section, entry, value);
                    OnChanging(e);
                    return !e.Cancel;
                }

                // Don't even bother if there are no handlers.
                if (Changed != null)
                    OnChanged(new ProfileChangedArgs(changeType, section, entry, value));
                return true;
            }

            /// <summary>
            ///   Raises the Changing event. </summary>
            /// <param name="e">
            ///   The arguments object associated with the Changing event. </param>
            /// <remarks>
            ///   This method should be invoked prior to making a change to the profile so that the
            ///   Changing event is raised, giving a chance to the handlers to prevent the change from
            ///   happening (by setting e.Cancel to true). This method calls each individual handler 
            ///   associated with the Changing event and checks the resulting e.Cancel flag.  
            ///   If it's true, it stops and does not call of any remaining handlers since the change 
            ///   needs to be prevented anyway. </remarks>
            /// <seealso cref="Changing" />
            /// <seealso cref="OnChanged" />
            protected virtual void OnChanging(ProfileChangingArgs e)
            {
                if (Changing == null)
                    return;

                foreach (ProfileChangingHandler handler in Changing.GetInvocationList())
                {
                    handler(this, e);

                    // If a particular handler cancels the event, stop
                    if (e.Cancel)
                        break;
                }
            }

            /// <summary>
            ///   Raises the Changed event. </summary>
            /// <param name="e">
            ///   The arguments object associated with the Changed event. </param>
            /// <remarks>
            ///   This method should be invoked after a change to the profile has been made so that the
            ///   Changed event is raised, giving a chance to the handlers to be notified of the change. </remarks>
            /// <seealso cref="Changed" />
            /// <seealso cref="OnChanging" />
            protected virtual void OnChanged(ProfileChangedArgs e)
            {
                if (Changed != null)
                    Changed(this, e);
            }


        }


        public class Ini : Profile
        {
            public Ini()
            {
            }

            public Ini(string fileName)
                : base(fileName)
            {
            }

            public Ini(Ini ini)
                : base(ini)
            {
            }
            /// <summary>
            ///   Gets the default name for the INI file. </summary>
            /// <remarks>
            ///   For Windows apps, this property returns the name of the executable plus .ini ("program.exe.ini").
            ///   For Web apps, this property returns the full path of <i>web.ini</i> based on the root folder.
            ///   This property is used to set the <see cref="Profile.Name" /> property inside the default constructor.</remarks>
            public override string DefaultName
            {
                get
                {
                    return DefaultNameWithoutExtension + ".ini";
                }
            }

            /// <summary>
            ///   Retrieves a copy of itself. </summary>
            /// <returns>
            ///   The return value is a copy of itself as an object. </returns>
            /// <seealso cref="Profile.CloneReadOnly" />
            public override object Clone()
            {
                return new Ini(this);
            }

            // The Win32 API methods
            [DllImport("kernel32", SetLastError = true)]
            static extern int WritePrivateProfileString(string section, string key, string value, string fileName);
            [DllImport("kernel32", SetLastError = true)]
            static extern int WritePrivateProfileString(string section, string key, int value, string fileName);
            [DllImport("kernel32", SetLastError = true)]
            static extern int WritePrivateProfileString(string section, int key, string value, string fileName);
            [DllImport("kernel32")]
            static extern int GetPrivateProfileString(string section, string key, string defaultValue, StringBuilder result, int size, string fileName);
            [DllImport("kernel32")]
            static extern int GetPrivateProfileString(string section, int key, string defaultValue, [MarshalAs(UnmanagedType.LPArray)] byte[] result, int size, string fileName);
            [DllImport("kernel32")]
            static extern int GetPrivateProfileString(int section, string key, string defaultValue, [MarshalAs(UnmanagedType.LPArray)] byte[] result, int size, string fileName);

            /// <summary>
            ///   Sets the value for an entry inside a section. </summary>
            /// <param name="section">
            ///   The name of the section that holds the entry. </param>
            /// <param name="entry">
            ///   The name of the entry where the value will be set. </param>
            /// <param name="value">
            ///   The value to set. If it's null, the entry is removed. </param>
            /// <exception cref="InvalidOperationException">
            ///   <see cref="Profile.ReadOnly" /> is true or
            ///   <see cref="Profile.Name" /> is null or empty. </exception>
            /// <exception cref="ArgumentNullException">
            ///   Either section or entry is null. </exception>
            /// <exception cref="Win32Exception">
            ///   The <see cref="WritePrivateProfileString" /> API failed. </exception>
            /// <remarks>
            ///   If the INI file does not exist, it is created.
            ///   The <see cref="Profile.Changing" /> event is raised before setting the value.  
            ///   If its <see cref="ProfileChangingArgs.Cancel" /> property is set to true, this method 
            ///   returns immediately without setting the value.  After the value has been set, 
            ///   the <see cref="Profile.Changed" /> event is raised. </remarks>
            /// <seealso cref="GetValue" />
            public override void SetValue(string section, string entry, object value)
            {
                // If the value is null, remove the entry
                if (value == null)
                {
                    RemoveEntry(section, entry);
                    return;
                }

                VerifyNotReadOnly();
                VerifyName();
                VerifyAndAdjustSection(ref section);
                VerifyAndAdjustEntry(ref entry);

                if (!RaiseChangeEvent(true, ProfileChangeType.SetValue, section, entry, value))
                    return;

                if (WritePrivateProfileString(section, entry, value.ToString(), Name) == 0)
                    throw new Win32Exception();

                RaiseChangeEvent(false, ProfileChangeType.SetValue, section, entry, value);
            }

            /// <summary>
            ///   Retrieves the value of an entry inside a section. </summary>
            /// <param name="section">
            ///   The name of the section that holds the entry with the value. </param>
            /// <param name="entry">
            ///   The name of the entry where the value is stored. </param>
            /// <returns>
            ///   The return value is the entry's value, or null if the entry does not exist. </returns>
            /// <exception cref="InvalidOperationException">
            ///	  <see cref="Profile.Name" /> is null or empty. </exception>
            /// <exception cref="ArgumentNullException">
            ///   Either section or entry is null. </exception>
            /// <seealso cref="SetValue" />
            /// <seealso cref="Profile.HasEntry" />
            public override object GetValue(string section, string entry)
            {
                VerifyName();
                VerifyAndAdjustSection(ref section);
                VerifyAndAdjustEntry(ref entry);

                // Loop until the buffer has grown enough to fit the value
                for (int maxSize = 250; true; maxSize *= 2)
                {
                    StringBuilder result = new StringBuilder(maxSize);
                    int size = GetPrivateProfileString(section, entry, "", result, maxSize, Name);

                    if (size < maxSize - 1)
                    {
                        if (size == 0 && !HasEntry(section, entry))
                            return null;
                        return result.ToString();
                    }
                }
            }

            /// <summary>
            ///   Removes an entry from a section. </summary>
            /// <param name="section">
            ///   The name of the section that holds the entry. </param>
            /// <param name="entry">
            ///   The name of the entry to remove. </param>
            /// <exception cref="InvalidOperationException">
            ///	  <see cref="Profile.Name" /> is null or empty or
            ///   <see cref="Profile.ReadOnly" /> is true. </exception>
            /// <exception cref="ArgumentNullException">
            ///   Either section or entry is null. </exception>
            /// <exception cref="Win32Exception">
            ///   The <see cref="WritePrivateProfileString" /> API failed. </exception>
            /// <remarks>
            ///   The <see cref="Profile.Changing" /> event is raised before removing the entry.  
            ///   If its <see cref="ProfileChangingArgs.Cancel" /> property is set to true, this method 
            ///   returns immediately without removing the entry.  After the entry has been removed, 
            ///   the <see cref="Profile.Changed" /> event is raised. </remarks>
            /// <seealso cref="RemoveSection" />
            public override void RemoveEntry(string section, string entry)
            {
                // Verify the entry exists
                if (!HasEntry(section, entry))
                    return;

                VerifyNotReadOnly();
                VerifyName();
                VerifyAndAdjustSection(ref section);
                VerifyAndAdjustEntry(ref entry);

                if (!RaiseChangeEvent(true, ProfileChangeType.RemoveEntry, section, entry, null))
                    return;

                if (WritePrivateProfileString(section, entry, 0, Name) == 0)
                    throw new Win32Exception();

                RaiseChangeEvent(false, ProfileChangeType.RemoveEntry, section, entry, null);
            }

            /// <summary>
            ///   Removes a section. </summary>
            /// <param name="section">
            ///   The name of the section to remove. </param>
            /// <exception cref="InvalidOperationException">
            ///	  <see cref="Profile.Name" /> is null or empty or
            ///   <see cref="Profile.ReadOnly" /> is true. </exception>
            /// <exception cref="ArgumentNullException">
            ///   section is null. </exception>
            /// <exception cref="Win32Exception">
            ///   The <see cref="WritePrivateProfileString" /> API failed. </exception>
            /// <remarks>
            ///   The <see cref="Profile.Changing" /> event is raised before removing the section.  
            ///   If its <see cref="ProfileChangingArgs.Cancel" /> property is set to true, this method 
            ///   returns immediately without removing the section.  After the section has been removed, 
            ///   the <see cref="Profile.Changed" /> event is raised. </remarks>
            /// <seealso cref="RemoveEntry" />
            public override void RemoveSection(string section)
            {
                // Verify the section exists
                if (!HasSection(section))
                    return;

                VerifyNotReadOnly();
                VerifyName();
                VerifyAndAdjustSection(ref section);

                if (!RaiseChangeEvent(true, ProfileChangeType.RemoveSection, section, null, null))
                    return;

                if (WritePrivateProfileString(section, 0, "", Name) == 0)
                    throw new Win32Exception();

                RaiseChangeEvent(false, ProfileChangeType.RemoveSection, section, null, null);
            }

            /// <summary>
            ///   Retrieves the names of all the entries inside a section. </summary>
            /// <param name="section">
            ///   The name of the section holding the entries. </param>
            /// <returns>
            ///   If the section exists, the return value is an array with the names of its entries; 
            ///   otherwise it's null. </returns>
            /// <exception cref="InvalidOperationException">
            ///	  <see cref="Profile.Name" /> is null or empty. </exception>
            /// <seealso cref="Profile.HasEntry" />
            /// <seealso cref="GetSectionNames" />
            public override string[] GetEntryNames(string section)
            {
                // Verify the section exists
                if (!HasSection(section))
                    return null;

                VerifyAndAdjustSection(ref section);

                // Loop until the buffer has grown enough to fit the value
                for (int maxSize = 500; true; maxSize *= 2)
                {
                    byte[] bytes = new byte[maxSize];
                    int size = GetPrivateProfileString(section, 0, "", bytes, maxSize, Name);

                    if (size < maxSize - 2)
                    {
                        // Convert the buffer to a string and split it
                        string entries = Encoding.ASCII.GetString(bytes, 0, size - (size > 0 ? 1 : 0));
                        if (entries == "")
                            return new string[0];
                        return entries.Split(new char[] { '\0' });
                    }
                }
            }

            /// <summary>
            ///   Retrieves the names of all the sections. </summary>
            /// <returns>
            ///   If the INI file exists, the return value is an array with the names of all the sections;
            ///   otherwise it's null. </returns>
            /// <seealso cref="Profile.HasSection" />
            /// <seealso cref="GetEntryNames" />
            public override string[] GetSectionNames()
            {
                // Verify the file exists
                if (!File.Exists(Name))
                    return null;

                // Loop until the buffer has grown enough to fit the value
                for (int maxSize = 500; true; maxSize *= 2)
                {
                    byte[] bytes = new byte[maxSize];
                    int size = GetPrivateProfileString(0, "", "", bytes, maxSize, Name);

                    if (size < maxSize - 2)
                    {
                        // Convert the buffer to a string and split it
                        string sections = Encoding.ASCII.GetString(bytes, 0, size - (size > 0 ? 1 : 0));
                        if (sections == "")
                            return new string[0];
                        return sections.Split(new char[] { '\0' });
                    }
                }
            }
        }
    }
}
