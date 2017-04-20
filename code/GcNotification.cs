GC.RegisterForFullGCNotification(maxGenerationThreshold,
  largeObjectHeapThreshold);
Task.Run(() => WaitForGC())
private static WaitForGC(){
  maxWaitMsec = 10000;
  bool didCollect = false;
  while(true){
    GCNotificationStatus status = 
      GC.WaitForFullGCApproach(maxWaitMsec);
    switch(status){
      case GCNotificationStatus.Succeeded:
        if(!didCollect){
          .. gc incoming..
          GC.Collect(2);
        }
        didCollect = !didCollect;
        break;
      case GCNotificationStatus.Canceled
        .. canceled
        break;
      case GCNotificationStatus.Timeout
        .. timeout
        break;
    }
  }
}