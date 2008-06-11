using System;
using System.Collections.Generic;
using System.Text;
using XmlIde.Editor.Stylers;
using XmlIde.Editor;
using IjwFramework.Types;

namespace XmlIde.LanguageFramework
{
	public class DataDrivenStyler : Styler
	{
		Parser parser;
		string filename;

		public override string Definition
		{
			get { return Config.GetAbsolutePath(filename); }
		}

		public DataDrivenStyler(string languageFilename)
		{
			filename = languageFilename;
			Reload();
		}

		public override IEnumerable<Span> GetStyles(Line line, int length)
		{
			CustomData<Checkpoint> lineCheckpoints = line.customData as CustomData<Checkpoint>;
			if (lineCheckpoints == null)
				line.customData = lineCheckpoints = new CustomData<Checkpoint>();

			if (lineCheckpoints.start == null)
				lineCheckpoints.start = parser.NewCheckpoint();

			Pair<IEnumerable<Span>, Checkpoint> p = 
				parser.Parse(line.Text.Substring(0, length), lineCheckpoints.start);

			lineCheckpoints.end = p.Second;
			return p.First;
		}

		protected override bool IsValidTransition(Line first, Line second)
		{
			CustomData<Checkpoint> firstData = first.customData as CustomData<Checkpoint>;
			CustomData<Checkpoint> secondData = second.customData as CustomData<Checkpoint>;

			if (CustomData<Checkpoint>.IsValidTransition(firstData, secondData))
				return true;

			second.customData = new CustomData<Checkpoint>(firstData.end);
			return false;
		}

		public override void Reload()
		{
			parser = ParserFactory.Load(Config.GetAbsolutePath(filename));
		}
	}
}
