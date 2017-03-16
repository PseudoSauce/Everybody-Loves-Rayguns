/*
 * An observer of a custom event must implement this interface in order to register to an event.
 * 
 * ie. Door : implements ICustomEventObserver {
 *      ...
 *      void notify(specific inherited message from ICustomEventHandler) {
 *          open();
 *      }
 *      ...
 * }
 * 
 * an observer of a specific event must register with the same event id as the invoker.
 * there can be multiple observers for one event.
 * 
 * the event id is the unique id
 * each observer of an eventID will trigger whatever event they are associated with,
 * when the receiving message is received through "Notify"
 */
public interface ICustomEventObserver {
    void Notify(CustomEventPacket handler);
}
