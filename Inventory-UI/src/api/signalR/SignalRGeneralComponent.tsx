import { useEffect, useState } from "react";
import * as signalR from "@microsoft/signalr";

export default function SignalRGeneralComponent({ hubName, eventListeners }: { hubName: string, eventListeners: SignalRComponentProps[] }) {
    useEffect(() => {
        const connection = new signalR.HubConnectionBuilder()
            .withUrl(`/hubs/general/${hubName}`)
            .build();
        
        connection.start()
            .then(() => console.log("Connection Started"))
            .catch((err: unknown) => console.error("Error while starting connection: ", err));
        
        eventListeners.forEach(({ hubListener, onMessageReceived }) => {
            connection.on(hubListener, (receivedMessage: string) => {
                onMessageReceived(receivedMessage);
            });
        });
    }, [])
}