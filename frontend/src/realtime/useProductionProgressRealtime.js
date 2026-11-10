import { useEffect, useRef } from "react";
import * as signalR from "@microsoft/signalr";

const API_BASE_URL =
  import.meta.env.VITE_API_BASE_URL ||
  import.meta.env.VITE_API_URL ||
  "https://localhost:7173"; // sửa đúng port API backend của bạn

export const useProductionProgressRealtime = ({ onProgress }) => {
  const onProgressRef = useRef(onProgress);

  useEffect(() => {
    onProgressRef.current = onProgress;
  }, [onProgress]);

  useEffect(() => {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`${API_BASE_URL}/hubs/production-progress`, {
        accessTokenFactory: () =>
          localStorage.getItem("token") ||
          localStorage.getItem("accessToken") ||
          ""
      })
      .withAutomaticReconnect([0, 2000, 5000, 10000])
      .configureLogging(signalR.LogLevel.Information)
      .build();

    const handleProgress = (dto) => {
      // console.log("REALTIME PROGRESS RECEIVED:", dto);
      onProgressRef.current?.(dto);
    };

    connection.on("workOrderProgressListUpdated", handleProgress);
    connection.on("workOrderProgressUpdated", handleProgress);

    connection
      .start()
      .then(() => {
        console.log("SignalR connected to production progress hub");
      })
      .catch((err) => {
        console.error("SignalR connect failed:", err);
      });

    return () => {
      connection.off("workOrderProgressListUpdated", handleProgress);
      connection.off("workOrderProgressUpdated", handleProgress);
      connection.stop();
    };
  }, []);
};