import React, { useState } from "react";
import { useAuth } from "auth";

/**
 * TODO: PoC only; REMOVE
 */

const ApiCall = () => {
  const { accessToken } = useAuth();
  const handleApiCallClick = async () => {
    const token = accessToken;
    let result;
    try {
      const response = await fetch("/identity", {
        headers: {
          Authorization: `Bearer ${token}`
        }
      });

      if (response.status === 200) {
        result = JSON.stringify(await response.json());
      } else {
        result = `API call failed with status: ${response.status}`;
      }
    } catch (e) {
      console.error(e);
      result = "API Call Failed";
    }
    setApiResult(result);
  };

  const [apiResult, setApiResult] = useState("");

  return (
    <>
      <button onClick={handleApiCallClick}>Attempt Api Call</button>
      <div>{apiResult}</div>
    </>
  );
};

export default ApiCall;
