using Yousei.Core;

namespace Yousei.Connectors.Nuxeo
{
    internal class NuxeoConnection : SimpleConnection
    {
        public NuxeoConnection(NuxeoConfig config)
        {
            AddAction("document_get", new DocumentGetAction(config));
            AddAction("request", new RequestAction(config));
            AddAction("batch_create", new BatchCreateAction(config));
            AddAction("batch_info", new BatchInfoAction(config));
            AddAction("batch_upload", new BatchUploadAction(config));
            AddAction("batch_drop", new BatchDropAction(config));
        }
    }
}