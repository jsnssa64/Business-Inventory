
interface SignalRComponentProps {
    hubListener: string;
    onMessageReceived: (message: string) => void;
}