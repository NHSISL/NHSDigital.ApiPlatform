import PatientView from "./PatientView";
import { patient } from "./patient";


function Viewer() {
    const resourceTypes = patient.entry.map(x => x.resource.resourceType);
    const counts = resourceTypes.reduce((acc, item) => {
        acc[item] = (acc[item] || 0) + 1;
        return acc;
    }, {})

    const patientEntry = patient.entry.filter(x => {
        if (x.resource)
            return x.resource.resourceType == "Patient";
        });

    return <> 
        {Object.entries(counts).map(([k, v]) => {
            return <div>{k} : {v}</div>
        })}
        <pre>
            <PatientView patient={patientEntry} />
        </pre>
    </>
    
}

export default Viewer;
