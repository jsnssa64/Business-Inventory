import { useEffect, useState } from "react";
import * as signalR from "@microsoft/signalr";

export default function SignalRSpecificComponent({ hubName, connectionId, eventListeners }: { hubName: string, connectionId: string, eventListeners: SignalRComponentProps[] }) {
    useEffect(() => {
        const connection = new signalR.HubConnectionBuilder()
            .withUrl(`/hubs/specific/${hubName}`)
            .build();

        connection.start()
            .then(() => {
                console.log("Connection Started");
                connection.invoke("JoinGroup", connectionId)
                    .then(() => {
                        console.log(`Joined group: ${connectionId}`);
                    })
                    .catch((err: unknown) => console.error("Error while joining group: ", err));
            })
            .catch((err: unknown) => console.error("Error while starting connection: ", err));
        
        eventListeners.forEach(({ hubListener, onMessageReceived }) => {
            connection.on(hubListener, (receivedMessage: string) => {
                onMessageReceived(receivedMessage);
            });
        });
    }, [])

    
}