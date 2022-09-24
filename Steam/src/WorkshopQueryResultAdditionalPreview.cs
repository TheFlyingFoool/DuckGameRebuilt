using System;

public class WorkshopQueryResultAdditionalPreview {
    public WorkshopQueryResultAdditionalPreview(bool isImage, string urlOrVideoID) {
        this.isImage = isImage;
        this.urlOrVideoID = urlOrVideoID;
    }

    public bool isImage;

    public string urlOrVideoID;
}
