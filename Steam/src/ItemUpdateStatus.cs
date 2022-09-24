using System;

public enum ItemUpdateStatus {
    CommittingChanges = 5,
    UploadingPreviewFile = 4,
    UploadingContent = 3,
    PreparingContent = 2,
    PreparingConfig = 1,
    Invalid = 0

}
