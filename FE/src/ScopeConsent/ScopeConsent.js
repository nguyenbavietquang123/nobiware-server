import { useEffect, useState } from "react";
import { useSearchParams } from "react-router-dom";

const ScopeConsent = () => {
  const [searchParams] = useSearchParams();
  const [scopes, setScopes] = useState([]);
  const [selectedScopes, setSelectedScopes] = useState([]);
  const [session, setSession] = useState(null);
  const [error, setError] = useState(null);
  // const responseType = searchParams.get("response_type");
  // const clientId = searchParams.get("client_id");
  // const redirectUri = searchParams.get("redirect_uri");
  // const state = searchParams.get("state");
  // const scopeParam = searchParams.get("scope");
  let scopeParam = "launch/patient openid fhirUser offline_access patient/Patient.rs patient/AllergyIntolerance.rs patient/CarePlan.rs patient/CareTeam.rs patient/Condition.rs patient/Coverage.rs patient/Device.rs patient/DiagnosticReport.rs patient/DocumentReference.rs patient/Encounter.rs patient/Goal.rs patient/Immunization.rs patient/Location.rs patient/Media.rs patient/Medication.rs patient/MedicationDispense.rs patient/MedicationRequest.rs patient/Observation.rs patient/Organization.rs patient/Practitioner.rs patient/PractitionerRole.rs patient/Procedure.rs patient/Provenance.rs patient/QuestionnaireResponse.rs patient/RelatedPerson.rs patient/ServiceRequest.rs patient/Specimen.rs";
  // const aud = searchParams.get("aud");
  // const code_challenge = searchParams.get("code_challenge");
  // const code_challenge_method = searchParams.get("code_challenge_method");
  const [allChecked, setAllChecked] = useState(true);

 
  useEffect(async () => {
    const params = new URLSearchParams(window.location.search);
    const proxyAuthBaseUrl = "http://192.168.56.1:5000";
    const session_id = params.get("session_id");
    const role = params.get("role");
    const code = params.get("code");
    if(role && role =="user"){
      scopeParam = "launch openid fhirUser offline_access user/Medication.rs user/AllergyIntolerance.rs user/CarePlan.rs user/CareTeam.rs user/Condition.rs user/Device.rs user/DiagnosticReport.rs user/DocumentReference.rs user/Encounter.rs user/Goal.rs user/Immunization.rs user/Location.rs user/MedicationRequest.rs user/Observation.rs user/Organization.rs user/Patient.rs user/Practitioner.rs user/Procedure.rs user/Provenance.rs user/PractitionerRole.rs user/ServiceRequest.rs user/Coverage.rs user/MedicationDispense.rs user/Specimen.rs" 
    }
    

    
    if (scopeParam) {
      const scopeList = scopeParam.split(" ");
      setScopes(scopeList);
      setSelectedScopes(scopeList); // all selected by default
    }
    if (session_id) {
      const newUrl = `${window.location.origin}/consent?session_id=${session_id}`;
      window.history.replaceState({}, '', newUrl);
    }
    
   
    fetch(`${proxyAuthBaseUrl}/auth/session?session_id=${session_id}`)
      .then((res) => {
        if (!res.ok) throw new Error('Session not found');
        return res.json();
      })
      .then((data) => setSession(data))
      .catch((err) => setError(err.message));
  }, []);

  const toggleScope = (scope) => {
    setSelectedScopes((prev) =>
      prev.includes(scope)
        ? prev.filter((s) => s !== scope)
        : [...prev, scope]
    );
  };

  const handleAuthorize = () => {

    const chosenScope = selectedScopes.join(" ");
    const authBaseUrl = "http://192.168.56.1:8080";
    const realm = "quang-fhir-server"; 
    console.log("authBaseUrl");
    console.log(authBaseUrl);
    console.log("realm");
    console.log(realm);
    const authEndpoint = `${authBaseUrl}/realms/${realm}/protocol/openid-connect/auth`; 
    const authorizeUrl = `${authEndpoint}?response_type=${response_type || ""}&client_id=${client_id || ""}&redirect_uri=${redirect_uri || ""}&scope=${chosenScope}&state=${state || ""}&aud=${aud}&code_challenge=${code_challenge}&code_challenge_method=${code_challenge_method}`;
    console.log("Authorize URL: ", authorizeUrl);
    //const authorizeUrl = `${authEndpoint}?response_type=${encodeURIComponent(responseType || "")}&client_id=${encodeURIComponent(clientId || "")}&redirect_uri=${encodeURIComponent(redirectUri || "")}&scope=${encodeURIComponent(chosenScope)}&state=${encodeURIComponent(state || "")}&aud=${encodeURIComponent(aud)}`;
    window.location.href = authorizeUrl;
  };
  const toggleAllScopes = () => {
    if (allChecked) {
      setSelectedScopes([]);
    } else {
      setSelectedScopes(scopes);
    }
    setAllChecked(!allChecked);
  };
  const handleCancel = () => {
    window.location.href = redirect_uri || "/";
  };
  if (error) return <p>Error: {error}</p>;
  if (!session) return <p>Loading...</p>;
  const {
    response_type,
    client_id,
    redirect_uri,
    scope,
    state,
    aud,
    code_challenge,
    code_challenge_method,
    role
  } = session;
  return (
    <div style={{ maxWidth: "500px", margin: "50px auto", padding: "20px", border: "1px solid #ccc", borderRadius: "10px" }}>
      <h2 style={{ textAlign: "center", marginBottom: "20px" }}>Authorize App Access</h2>
      <p>Select which scopes you'd like to grant:</p>
      <button
        onClick={toggleAllScopes}
        style={{ marginBottom: "10px", padding: "5px 10px", backgroundColor: "#f3f4f6", border: "1px solid #ccc", borderRadius: "5px" }}
      >
        {allChecked ? "Uncheck All" : "Check All"}
      </button>
      <ul style={{ listStyle: "none", padding: 0 }}>
        {scopes.map((scope) => (
          <li key={scope} style={{ marginBottom: "10px" }}>
            <label>
              <input
                type="checkbox"
                checked={selectedScopes.includes(scope)}
                onChange={() => toggleScope(scope)}
              />{" "}
              {scope}
            </label>
          </li>
        ))}
      </ul>
      <button
        onClick={handleAuthorize}
        style={{
          marginTop: "20px",
          width: "100%",
          padding: "10px",
          backgroundColor: "#4f46e5",
          color: "white",
          border: "none",
          borderRadius: "5px",
          cursor: "pointer",
        }}
      >
        Authorize Selected Scopes
      </button>
      <button
        onClick={handleCancel}
        style={{
          marginTop: "10px",
          width: "100%",
          padding: "10px",
          backgroundColor: "#e5e7eb",
          color: "#111827",
          border: "none",
          borderRadius: "5px",
          cursor: "pointer",
        }}
      >
        Cancel
      </button>
    </div>
  );
};

export default ScopeConsent;
