import { useEffect, useState } from 'react'
import Viewer from './viewer'

function App() {

    const [tokens, setTokens] = useState();
    useEffect(() => {
        var r = fetch('/auth/session').then(d => d.json()).then(r => setTokens(r));
    },[]);


    const logout = () => {
        fetch('/auth/logout', { method: "POST" });
    }

  return (
      <>
          <a href="/auth/login">login</a>
          <br/>
          <a href="/api/patient">Patient</a>
          <button onClick={logout}>Logout</button>
          <pre>{JSON.stringify(tokens, null, 2)}</pre>

    </>
  )
}

export default App
