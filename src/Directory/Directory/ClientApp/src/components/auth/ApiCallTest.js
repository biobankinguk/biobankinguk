import React, { useState } from "react";
import authorizeService from "../../services/authorize-service";

/**
 * TODO: PoC only; REMOVE
 */

const ApiCall = () => {
  const handleApiCallClick = async () => {
    const token = await authorizeService.getAccessToken();
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
