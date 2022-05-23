using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Xml.Linq;

namespace Usporedi
{
    class Program
    {
        static void Main(string[] args)
        {
            var prvii = XDocument.Parse(prvi.xml);
            var drugii = XDocument.Parse(drugi.xml);
            // helper function to get the attribute value of the given element by attribute name
            Func<XElement, string, string> getAttributeValue = (xElement, name) => xElement.Attribute(name).Value;
            // nodes for which we are looking for
            var nodeName = "book";
            var sameNodes = new List<string>();
            // iterate over all old nodes (this will replace all existing but changed nodes)
            prvii.Descendants(nodeName).ToList().ForEach(item =>
            {
                var currentElementId = getAttributeValue(item, "id");
                // find node with the same id in the new nodes collection
                var toReplace = xmlNew.Descendants(nodeName).ToList().FirstOrDefault(n => getAttributeValue(n, "id") == currentElementId);
                if (toReplace != null)
                {
                    var aImageOldValue = getAttributeValue(item, "image");
                    var aImageNewValue = getAttributeValue(toReplace, "image");
                    var aNameOldValue = getAttributeValue(item, "name");
                    var aNameNewValue = getAttributeValue(toReplace, "name");
                    if ((aImageNewValue != aImageOldValue) || (aNameOldValue != aNameNewValue))
                    {
                        // replace attribute values
                        item.Attribute("image").Value = getAttributeValue(toReplace, "image");
                        item.Attribute("name").Value = getAttributeValue(toReplace, "name");
                    }
                    else if ((aImageNewValue == aImageOldValue) && (aNameOldValue == aNameNewValue))
                    {
                        // remove same nodes! can't remove the node yet, because it will be seen as new
                        sameNodes.Add(getAttributeValue(item, "id"));
                    }
                }
            });
            // add new nodes
            // id's of all old nodes
            var oldNodes = prvii.Descendants(nodeName).Select(node => getAttributeValue(node, "id")).ToList();
            // id's of all new nodes
            var newNodes = drugii.Descendants(nodeName).Select(node => getAttributeValue(node, "id")).ToList();
            // find new nodes that are not present in the old collection
            var nodeIdsToAdd = newNodes.Except(oldNodes);
            // add all new nodes to the already modified xml document
            foreach (var newNodeId in nodeIdsToAdd)
            {
                var newNode = drugii.Descendants(nodeName).FirstOrDefault(node => getAttributeValue(node, "id") == newNodeId);
                if (newNode != null)
                {
                    prvii.Root.Add(newNode);
                }
            }
            // remove unchanged nodes
            foreach (var oldNodeId in sameNodes)
            {
                prvii.Descendants(nodeName).FirstOrDefault(node => getAttributeValue(node, "id") == oldNodeId).Remove();
            }
            prvii.Save(@"d:\temp\merged.xml");
        }
    }
}
