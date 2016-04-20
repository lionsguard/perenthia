using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

using Radiance.Commands;

namespace Radiance.Configuration
{
	/// <summary>
	/// Provides a class the represents the world configuration file.
	/// </summary>
	public class WorldConfig
	{
		/// <summary>
		/// Gets or sets a value indicating whether or not magic is enabled for the virtual world.
		/// </summary>
		public bool EnableMagic { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether or not psionics are enabled for the virtual world.
		/// </summary>
		public bool EnablePsionics { get; set; }

		/// <summary>
		/// Gets or sets a value indicating how deadly combat is; a value of 2 is the default, 1 is real to life and 3 is larger than life.
		/// </summary>
		public int RealismMultiplier { get; set; }

		/// <summary>
		/// Gets or sets the amount of magic used in the game; a value of 2 is the default, 1 requires players to use magic 
		/// more sparingly and 3 will allow them to use powers more than they probably should.
		/// </summary>
		public int PowerMultiplier { get; set; }

		/// <summary>
		/// Gets or sets the Radiance.World derived type that will drive the current virtual world.
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		/// Gets or sets the name of the virtual world.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets a dictionary of skill name/description values for all the skills available in the virtual world.
		/// </summary>
		public SkillDictionary Skills { get; private set; }

		/// <summary>
		/// Gets a dictionary of commands available in the virtual world.
		/// </summary>
		public CommandDictionary Commands { get; private set; }


		/// <summary>
		/// Initializes a new instance of the WorldConfig class with the world configuration file path specified.
		/// </summary>
		/// <param name="filePath">The path to the world configuration file.</param>
		public WorldConfig(string filePath)
		{
			this.Skills = new SkillDictionary();
			this.Commands = new CommandDictionary();

			if (!String.IsNullOrEmpty(filePath))
			{
				if (File.Exists(filePath))
				{
					XmlDocument doc = new XmlDocument();
					doc.Load(filePath);

					if (doc.HasChildNodes)
					{
						this.EnableMagic = Convert.ToBoolean(doc.DocumentElement.Attributes["enableMagic"].Value);
						this.EnablePsionics = Convert.ToBoolean(doc.DocumentElement.Attributes["enablePsionics"].Value);
						this.RealismMultiplier = Convert.ToInt32(doc.DocumentElement.Attributes["realismMultiplier"].Value);
						this.PowerMultiplier = Convert.ToInt32(doc.DocumentElement.Attributes["powerMultiplier"].Value);
						this.Name = doc.DocumentElement.Attributes["name"].Value;
						this.Type = doc.DocumentElement.Attributes["type"].Value;

						// Skills
						XmlNodeList skills = doc.DocumentElement.SelectNodes("skills/add");
						for (int i = 0; i < skills.Count; i++)
						{
							this.Skills.Add(skills[i].Attributes["name"].Value, skills[i].InnerText);
						}

						// Commands
						XmlNodeList commands = doc.DocumentElement.SelectNodes("commands/add");
						for (int i = 0; i < commands.Count; i++)
						{
							Command cmd = Activator.CreateInstance(System.Type.GetType(commands[i].Attributes["type"].Value)) as Command;
							if (cmd != null)
							{
								this.Commands.Add(commands[i].Attributes["name"].Value, cmd);
							}
						}
					}
				}
			}
		}
	}
}
