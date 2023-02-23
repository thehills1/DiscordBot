using System.Collections;
using System.Collections.ObjectModel;
using DiscordBot.Database.Tables;

namespace DiscordBot.Extensions.Collections
{
	public class TableCollection<T> : ReadOnlyCollection<T>, ITableCollection where T : BaseTable
	{
		BaseTable ITableCollection.this[int index] { get => this[index]; }

		public TableCollection(IList<T> list) : base(list) { }

		public Type GetTableType() => typeof(T);

		public new IEnumerator<BaseTable> GetEnumerator()
		{
			return base.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return base.GetEnumerator();
		}
	}
}
