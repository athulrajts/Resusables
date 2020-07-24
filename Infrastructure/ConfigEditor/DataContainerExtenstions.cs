using ConfigEditor.Models;
using KEI.Infrastructure.Configuration;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ConfigEditor
{
    public static class DataContainerExtenstions
    {
        public static TreeNodeModel ToTreeNode(this IPropertyContainer dc) => new TreeNodeModel(dc);
    }
}
