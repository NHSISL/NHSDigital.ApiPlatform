export const patient = {
    "entry": [
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "1711"
                    },
                    {
                        "extension": [
                            {
                                "valueCodeableConcept": {
                                    "coding": [
                                        {
                                            "system": "https://fhir.hl7.org.uk/CareConnect-NHSNumberVerificationStatus-1",
                                            "code": "01",
                                            "display": "Number present and verified"
                                        }
                                    ]
                                },
                                "url": "https://fhir.hl7.org.uk/STU3/StructureDefinition/Extension-CareConnect-NHSNumberVerificationStatus-1"
                            }
                        ],
                        "system": "https://fhir.hl7.org.uk/Id/nhs-number",
                        "value": "5558526785"
                    }
                ],
                "extension": [
                    {
                        "extension": [
                            {
                                "valuePeriod": {
                                    "start": "2013-05-29T00:00:00+00:00"
                                },
                                "url": "registrationPeriod"
                            },
                            {
                                "valueCodeableConcept": {
                                    "coding": [
                                        {
                                            "system": "https://fhir.hl7.org.uk/STU3/ValueSet/CareConnect-RegistrationType-1",
                                            "display": "Regular/GMS"
                                        }
                                    ]
                                },
                                "url": "registrationType"
                            }
                        ],
                        "url": "https://fhir.hl7.org.uk/STU3/StructureDefinition/Extension-CareConnect-RegistrationDetails-1"
                    }
                ],
                "address": [
                    {
                        "city": "Truro",
                        "use": "home",
                        "line": [
                            "Road"
                        ]
                    }
                ],
                "gender": "male",
                "birthDate": "1970-01-01",
                "managingOrganization": {
                    "reference": "Organization/5ff06392-92cb-4e43-a4cf-d7d683d09197"
                },
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-Patient-1"
                    ]
                },
                "generalPractitioner": [
                    {
                        "reference": "Practitioner/75bc169c-df60-41d5-9782-5e785529eb40"
                    }
                ],
                "name": [
                    {
                        "given": [
                            "John"
                        ],
                        "use": "official",
                        "prefix": [
                            "Mr"
                        ],
                        "family": "Smith"
                    }
                ],
                "telecom": [
                    {
                        "system": "phone",
                        "use": "mobile",
                        "value": "02083456788"
                    }
                ],
                "id": "bf3904da-c11f-4004-a774-f6049cb8308e",
                "communication": [
                    {
                        "language": {
                            "coding": [
                                {
                                    "code": "13lS.",
                                    "display": "Main spoken language Albanian"
                                }
                            ]
                        }
                    }
                ],
                "resourceType": "Patient"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73520"
                    }
                ],
                "extension": [
                    {
                        "valueCodeableConcept": {
                            "coding": [
                                {
                                    "system": "http://endeavourhealth.org/fhir/ValueSet/primarycare-problem-significance",
                                    "display": "Significant (qualifier value)"
                                }
                            ]
                        },
                        "url": "http://endeavourhealth.org/fhir/StructureDefinition/primarycare-problem-significance-extension"
                    }
                ],
                "code": {
                    "coding": [
                        {
                            "system": "http://snomed.info/sct",
                            "code": "TJ961",
                            "display": "AR - Lysergide - LSD"
                        }
                    ],
                    "text": "AR - Lysergide - LSD"
                },
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-ProblemHeader-Condition-1"
                    ],
                    "tag": [
                        {
                            "system": "https://fhir.nhs.uk/Id/ODS-Code",
                            "code": "EMIS99",
                            "display": "GPES Org 20077"
                        }
                    ]
                },
                "subject": {
                    "reference": "Patient/bf3904da-c11f-4004-a774-f6049cb8308e"
                },
                "id": "3f1962be-d1f6-40fa-9f4e-23689cc928bc",
                "clinicalStatus": "active",
                "category": [
                    {
                        "coding": [
                            {
                                "system": "http://terminology.hl7.org/CodeSystem/condition-category",
                                "code": "problem-list-item",
                                "display": "Problem list Item"
                            }
                        ]
                    }
                ],
                "onsetDateTime": "2014-01-30T00:00:00+00:00",
                "resourceType": "Condition"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73523"
                    }
                ],
                "extension": [
                    {
                        "valueCodeableConcept": {
                            "coding": [
                                {
                                    "system": "http://endeavourhealth.org/fhir/ValueSet/primarycare-problem-significance",
                                    "display": "Not significant (qualifier value)"
                                }
                            ]
                        },
                        "url": "http://endeavourhealth.org/fhir/StructureDefinition/primarycare-problem-significance-extension"
                    }
                ],
                "code": {
                    "coding": [
                        {
                            "system": "http://snomed.info/sct",
                            "code": "1B1G.",
                            "display": "Headache"
                        }
                    ],
                    "text": "Headache"
                },
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-ProblemHeader-Condition-1"
                    ],
                    "tag": [
                        {
                            "system": "https://fhir.nhs.uk/Id/ODS-Code",
                            "code": "EMIS99",
                            "display": "GPES Org 20077"
                        }
                    ]
                },
                "subject": {
                    "reference": "Patient/bf3904da-c11f-4004-a774-f6049cb8308e"
                },
                "id": "45cd5f93-2305-47a9-bb72-e33348faf9ba",
                "clinicalStatus": "resolved",
                "category": [
                    {
                        "coding": [
                            {
                                "system": "http://terminology.hl7.org/CodeSystem/condition-category",
                                "code": "problem-list-item",
                                "display": "Problem list Item"
                            }
                        ]
                    }
                ],
                "onsetDateTime": "2013-12-20T00:00:00+00:00",
                "resourceType": "Condition"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73527"
                    }
                ],
                "extension": [
                    {
                        "valueCodeableConcept": {
                            "coding": [
                                {
                                    "system": "http://endeavourhealth.org/fhir/ValueSet/primarycare-problem-significance",
                                    "display": "Significant (qualifier value)"
                                }
                            ]
                        },
                        "url": "http://endeavourhealth.org/fhir/StructureDefinition/primarycare-problem-significance-extension"
                    }
                ],
                "code": {
                    "coding": [
                        {
                            "system": "http://snomed.info/sct",
                            "code": "13M4.",
                            "display": "Death Of Pet"
                        }
                    ],
                    "text": "Death Of Pet"
                },
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-ProblemHeader-Condition-1"
                    ],
                    "tag": [
                        {
                            "system": "https://fhir.nhs.uk/Id/ODS-Code",
                            "code": "EMIS99",
                            "display": "GPES Org 20077"
                        }
                    ]
                },
                "subject": {
                    "reference": "Patient/bf3904da-c11f-4004-a774-f6049cb8308e"
                },
                "id": "d4124ddb-6c9d-436c-9d12-5694076694e5",
                "clinicalStatus": "resolved",
                "category": [
                    {
                        "coding": [
                            {
                                "system": "http://terminology.hl7.org/CodeSystem/condition-category",
                                "code": "problem-list-item",
                                "display": "Problem list Item"
                            }
                        ]
                    }
                ],
                "onsetDateTime": "2013-12-05T00:00:00+00:00",
                "resourceType": "Condition"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73531"
                    }
                ],
                "extension": [
                    {
                        "valueCodeableConcept": {
                            "coding": [
                                {
                                    "system": "http://endeavourhealth.org/fhir/ValueSet/primarycare-problem-significance",
                                    "display": "Significant (qualifier value)"
                                }
                            ]
                        },
                        "url": "http://endeavourhealth.org/fhir/StructureDefinition/primarycare-problem-significance-extension"
                    }
                ],
                "code": {
                    "coding": [
                        {
                            "system": "http://snomed.info/sct",
                            "code": "EMISNOFH5",
                            "display": "No FH: Diabetes"
                        }
                    ],
                    "text": "No FH: Diabetes"
                },
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-ProblemHeader-Condition-1"
                    ],
                    "tag": [
                        {
                            "system": "https://fhir.nhs.uk/Id/ODS-Code",
                            "code": "EMIS99",
                            "display": "GPES Org 20077"
                        }
                    ]
                },
                "subject": {
                    "reference": "Patient/bf3904da-c11f-4004-a774-f6049cb8308e"
                },
                "id": "ee51adf7-77ee-4af7-9fe6-d66cd9e6916d",
                "clinicalStatus": "resolved",
                "category": [
                    {
                        "coding": [
                            {
                                "system": "http://terminology.hl7.org/CodeSystem/condition-category",
                                "code": "problem-list-item",
                                "display": "Problem list Item"
                            }
                        ]
                    }
                ],
                "onsetDateTime": "2013-12-05T00:00:00+00:00",
                "resourceType": "Condition"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73533"
                    }
                ],
                "extension": [
                    {
                        "valueCodeableConcept": {
                            "coding": [
                                {
                                    "system": "http://endeavourhealth.org/fhir/ValueSet/primarycare-problem-significance",
                                    "display": "Significant (qualifier value)"
                                }
                            ]
                        },
                        "url": "http://endeavourhealth.org/fhir/StructureDefinition/primarycare-problem-significance-extension"
                    }
                ],
                "code": {
                    "coding": [
                        {
                            "system": "http://snomed.info/sct",
                            "code": "SN52.",
                            "display": "Drug Hypersensitivity NOS"
                        }
                    ],
                    "text": "Drug Hypersensitivity NOS"
                },
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-ProblemHeader-Condition-1"
                    ],
                    "tag": [
                        {
                            "system": "https://fhir.nhs.uk/Id/ODS-Code",
                            "code": "EMIS99",
                            "display": "GPES Org 20077"
                        }
                    ]
                },
                "subject": {
                    "reference": "Patient/bf3904da-c11f-4004-a774-f6049cb8308e"
                },
                "id": "4ead2ff8-1ca5-4109-9d9b-906506838214",
                "clinicalStatus": "resolved",
                "category": [
                    {
                        "coding": [
                            {
                                "system": "http://terminology.hl7.org/CodeSystem/condition-category",
                                "code": "problem-list-item",
                                "display": "Problem list Item"
                            }
                        ]
                    }
                ],
                "onsetDateTime": "2013-12-04T00:00:00+00:00",
                "resourceType": "Condition"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73537"
                    }
                ],
                "extension": [
                    {
                        "valueCodeableConcept": {
                            "coding": [
                                {
                                    "system": "http://endeavourhealth.org/fhir/ValueSet/primarycare-problem-significance",
                                    "display": "Significant (qualifier value)"
                                }
                            ]
                        },
                        "url": "http://endeavourhealth.org/fhir/StructureDefinition/primarycare-problem-significance-extension"
                    }
                ],
                "code": {
                    "coding": [
                        {
                            "system": "http://snomed.info/sct",
                            "code": "TJ53.",
                            "display": "AR - Salicylates"
                        }
                    ],
                    "text": "AR - Salicylates"
                },
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-ProblemHeader-Condition-1"
                    ],
                    "tag": [
                        {
                            "system": "https://fhir.nhs.uk/Id/ODS-Code",
                            "code": "EMIS99",
                            "display": "GPES Org 20077"
                        }
                    ]
                },
                "subject": {
                    "reference": "Patient/bf3904da-c11f-4004-a774-f6049cb8308e"
                },
                "id": "2346ffa1-1aa5-4057-98ae-8945abd6fe55",
                "clinicalStatus": "active",
                "category": [
                    {
                        "coding": [
                            {
                                "system": "http://terminology.hl7.org/CodeSystem/condition-category",
                                "code": "problem-list-item",
                                "display": "Problem list Item"
                            }
                        ]
                    }
                ],
                "onsetDateTime": "2013-12-04T00:00:00+00:00",
                "resourceType": "Condition"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73539"
                    }
                ],
                "extension": [
                    {
                        "valueCodeableConcept": {
                            "coding": [
                                {
                                    "system": "http://endeavourhealth.org/fhir/ValueSet/primarycare-problem-significance",
                                    "display": "Not significant (qualifier value)"
                                }
                            ]
                        },
                        "url": "http://endeavourhealth.org/fhir/StructureDefinition/primarycare-problem-significance-extension"
                    }
                ],
                "code": {
                    "coding": [
                        {
                            "system": "http://snomed.info/sct",
                            "code": "182..",
                            "display": "Chest Pain"
                        }
                    ],
                    "text": "Chest Pain"
                },
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-ProblemHeader-Condition-1"
                    ],
                    "tag": [
                        {
                            "system": "https://fhir.nhs.uk/Id/ODS-Code",
                            "code": "EMIS99",
                            "display": "GPES Org 20077"
                        }
                    ]
                },
                "subject": {
                    "reference": "Patient/bf3904da-c11f-4004-a774-f6049cb8308e"
                },
                "id": "c8dcfb0e-5290-4e1a-a2cf-091aa681e1f8",
                "clinicalStatus": "active",
                "category": [
                    {
                        "coding": [
                            {
                                "system": "http://terminology.hl7.org/CodeSystem/condition-category",
                                "code": "problem-list-item",
                                "display": "Problem list Item"
                            }
                        ]
                    }
                ],
                "onsetDateTime": "2014-02-18T00:00:00+00:00",
                "resourceType": "Condition"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73545"
                    }
                ],
                "extension": [
                    {
                        "valueCodeableConcept": {
                            "coding": [
                                {
                                    "system": "http://endeavourhealth.org/fhir/ValueSet/primarycare-problem-significance",
                                    "display": "Significant (qualifier value)"
                                }
                            ]
                        },
                        "url": "http://endeavourhealth.org/fhir/StructureDefinition/primarycare-problem-significance-extension"
                    }
                ],
                "code": {
                    "coding": [
                        {
                            "system": "http://snomed.info/sct",
                            "code": "SN580",
                            "display": "Egg Allergy"
                        }
                    ],
                    "text": "Egg Allergy"
                },
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-ProblemHeader-Condition-1"
                    ],
                    "tag": [
                        {
                            "system": "https://fhir.nhs.uk/Id/ODS-Code",
                            "code": "EMIS99",
                            "display": "GPES Org 20077"
                        }
                    ]
                },
                "subject": {
                    "reference": "Patient/bf3904da-c11f-4004-a774-f6049cb8308e"
                },
                "id": "5b331764-353a-4583-a0b1-5cf6e82316c0",
                "clinicalStatus": "active",
                "category": [
                    {
                        "coding": [
                            {
                                "system": "http://terminology.hl7.org/CodeSystem/condition-category",
                                "code": "problem-list-item",
                                "display": "Problem list Item"
                            }
                        ]
                    }
                ],
                "onsetDateTime": "2014-01-30T00:00:00+00:00",
                "resourceType": "Condition"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73548"
                    }
                ],
                "extension": [
                    {
                        "valueCodeableConcept": {
                            "coding": [
                                {
                                    "system": "http://endeavourhealth.org/fhir/ValueSet/primarycare-problem-significance",
                                    "display": "Not significant (qualifier value)"
                                }
                            ]
                        },
                        "url": "http://endeavourhealth.org/fhir/StructureDefinition/primarycare-problem-significance-extension"
                    }
                ],
                "code": {
                    "coding": [
                        {
                            "system": "http://snomed.info/sct",
                            "code": "TG808",
                            "display": "Accid.-scald-chocolate"
                        }
                    ],
                    "text": "Accid.-scald-chocolate"
                },
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-ProblemHeader-Condition-1"
                    ],
                    "tag": [
                        {
                            "system": "https://fhir.nhs.uk/Id/ODS-Code",
                            "code": "EMIS99",
                            "display": "GPES Org 20077"
                        }
                    ]
                },
                "subject": {
                    "reference": "Patient/bf3904da-c11f-4004-a774-f6049cb8308e"
                },
                "id": "0472a4f2-7470-40da-a5c0-42400c8a98a7",
                "clinicalStatus": "resolved",
                "category": [
                    {
                        "coding": [
                            {
                                "system": "http://terminology.hl7.org/CodeSystem/condition-category",
                                "code": "problem-list-item",
                                "display": "Problem list Item"
                            }
                        ]
                    }
                ],
                "onsetDateTime": "2013-12-05T00:00:00+00:00",
                "resourceType": "Condition"
            }
        },
        {
            "resource": {
                "mode": "snapshot",
                "date": "2026-01-23T11:00:18+00:00",
                "entry": [
                    {
                        "item": {
                            "reference": "Condition/3f1962be-d1f6-40fa-9f4e-23689cc928bc"
                        }
                    },
                    {
                        "item": {
                            "reference": "Condition/45cd5f93-2305-47a9-bb72-e33348faf9ba"
                        }
                    },
                    {
                        "item": {
                            "reference": "Condition/d4124ddb-6c9d-436c-9d12-5694076694e5"
                        }
                    },
                    {
                        "item": {
                            "reference": "Condition/ee51adf7-77ee-4af7-9fe6-d66cd9e6916d"
                        }
                    },
                    {
                        "item": {
                            "reference": "Condition/4ead2ff8-1ca5-4109-9d9b-906506838214"
                        }
                    },
                    {
                        "item": {
                            "reference": "Condition/2346ffa1-1aa5-4057-98ae-8945abd6fe55"
                        }
                    },
                    {
                        "item": {
                            "reference": "Condition/c8dcfb0e-5290-4e1a-a2cf-091aa681e1f8"
                        }
                    },
                    {
                        "item": {
                            "reference": "Condition/5b331764-353a-4583-a0b1-5cf6e82316c0"
                        }
                    },
                    {
                        "item": {
                            "reference": "Condition/0472a4f2-7470-40da-a5c0-42400c8a98a7"
                        }
                    }
                ],
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-List-1"
                    ]
                },
                "subject": {
                    "reference": "Patient/bf3904da-c11f-4004-a774-f6049cb8308e"
                },
                "title": "Problems",
                "resourceType": "List",
                "status": "current"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73962"
                    }
                ],
                "code": {
                    "coding": [
                        {
                            "system": "http://snomed.info/sct",
                            "code": "TJ961",
                            "display": "AR - Lysergide - LSD"
                        }
                    ]
                },
                "asserter": {
                    "reference": "PractitionerRole/89adffa3-0342-407e-9f65-4f2ff39cfebf"
                },
                "verificationStatus": "confirmed",
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-AllergyIntolerance-1"
                    ],
                    "tag": [
                        {
                            "system": "https://fhir.nhs.uk/Id/ODS-Code",
                            "code": "EMIS99",
                            "display": "GPES Org 20077"
                        }
                    ]
                },
                "id": "675225a6-aa26-415a-948e-def17bc45e53",
                "clinicalStatus": "active",
                "type": "allergy",
                "onsetDateTime": "2014-01-29T00:00:00+00:00",
                "resourceType": "AllergyIntolerance"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73963"
                    }
                ],
                "code": {
                    "coding": [
                        {
                            "system": "http://snomed.info/sct",
                            "code": "SN589",
                            "display": "Allergy To Strawberries"
                        }
                    ]
                },
                "asserter": {
                    "reference": "PractitionerRole/89adffa3-0342-407e-9f65-4f2ff39cfebf"
                },
                "verificationStatus": "confirmed",
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-AllergyIntolerance-1"
                    ],
                    "tag": [
                        {
                            "system": "https://fhir.nhs.uk/Id/ODS-Code",
                            "code": "EMIS99",
                            "display": "GPES Org 20077"
                        }
                    ]
                },
                "id": "6c0d05c8-f499-479c-a33f-9c4d9a650671",
                "clinicalStatus": "active",
                "type": "allergy",
                "onsetDateTime": "2014-01-30T00:00:00+00:00",
                "resourceType": "AllergyIntolerance"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73964"
                    }
                ],
                "code": {
                    "coding": [
                        {
                            "system": "http://snomed.info/sct",
                            "code": "SN580",
                            "display": "Egg Allergy"
                        }
                    ]
                },
                "asserter": {
                    "reference": "PractitionerRole/89adffa3-0342-407e-9f65-4f2ff39cfebf"
                },
                "verificationStatus": "confirmed",
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-AllergyIntolerance-1"
                    ],
                    "tag": [
                        {
                            "system": "https://fhir.nhs.uk/Id/ODS-Code",
                            "code": "EMIS99",
                            "display": "GPES Org 20077"
                        }
                    ]
                },
                "id": "5e1bfa2e-c2d7-4ff2-b496-5a76121da661",
                "clinicalStatus": "active",
                "type": "allergy",
                "onsetDateTime": "2014-01-30T00:00:00+00:00",
                "resourceType": "AllergyIntolerance"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73965"
                    }
                ],
                "code": {
                    "coding": [
                        {
                            "system": "http://snomed.info/sct",
                            "code": "TJ961",
                            "display": "AR - Lysergide - LSD"
                        }
                    ]
                },
                "asserter": {
                    "reference": "PractitionerRole/89adffa3-0342-407e-9f65-4f2ff39cfebf"
                },
                "verificationStatus": "confirmed",
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-AllergyIntolerance-1"
                    ],
                    "tag": [
                        {
                            "system": "https://fhir.nhs.uk/Id/ODS-Code",
                            "code": "EMIS99",
                            "display": "GPES Org 20077"
                        }
                    ]
                },
                "id": "e57a0b61-741a-4734-b372-dee53b0169e4",
                "clinicalStatus": "active",
                "type": "allergy",
                "onsetDateTime": "2014-01-30T00:00:00+00:00",
                "resourceType": "AllergyIntolerance"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73966"
                    }
                ],
                "code": {
                    "coding": [
                        {
                            "system": "http://snomed.info/sct",
                            "code": "SN583",
                            "display": "Nut Allergy"
                        }
                    ]
                },
                "asserter": {
                    "reference": "PractitionerRole/89adffa3-0342-407e-9f65-4f2ff39cfebf"
                },
                "verificationStatus": "confirmed",
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-AllergyIntolerance-1"
                    ],
                    "tag": [
                        {
                            "system": "https://fhir.nhs.uk/Id/ODS-Code",
                            "code": "EMIS99",
                            "display": "GPES Org 20077"
                        }
                    ]
                },
                "id": "b6e88275-0a9b-4a92-90bb-54e6e5bd5938",
                "clinicalStatus": "active",
                "type": "allergy",
                "onsetDateTime": "2014-01-30T00:00:00+00:00",
                "resourceType": "AllergyIntolerance"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73967"
                    }
                ],
                "code": {
                    "coding": [
                        {
                            "system": "http://snomed.info/sct",
                            "code": "SN52.",
                            "display": "Drug Hypersensitivity NOS"
                        }
                    ]
                },
                "asserter": {
                    "reference": "PractitionerRole/66d19ac6-ce1b-4623-b860-372634dca807"
                },
                "verificationStatus": "confirmed",
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-AllergyIntolerance-1"
                    ],
                    "tag": [
                        {
                            "system": "https://fhir.nhs.uk/Id/ODS-Code",
                            "code": "EMIS99",
                            "display": "GPES Org 20077"
                        }
                    ]
                },
                "id": "e8565f72-9701-478f-abe3-d88471b8a51a",
                "clinicalStatus": "active",
                "type": "allergy",
                "onsetDateTime": "2013-12-05T00:00:00+00:00",
                "resourceType": "AllergyIntolerance"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73968"
                    }
                ],
                "code": {
                    "coding": [
                        {
                            "system": "http://snomed.info/sct",
                            "code": "TJ961",
                            "display": "AR - Lysergide - LSD"
                        }
                    ]
                },
                "asserter": {
                    "reference": "PractitionerRole/89adffa3-0342-407e-9f65-4f2ff39cfebf"
                },
                "verificationStatus": "confirmed",
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-AllergyIntolerance-1"
                    ],
                    "tag": [
                        {
                            "system": "https://fhir.nhs.uk/Id/ODS-Code",
                            "code": "EMIS99",
                            "display": "GPES Org 20077"
                        }
                    ]
                },
                "id": "c3e0b81c-ecf4-4865-ac02-24f02ab839d2",
                "clinicalStatus": "active",
                "type": "allergy",
                "onsetDateTime": "2014-01-29T00:00:00+00:00",
                "resourceType": "AllergyIntolerance"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73969"
                    }
                ],
                "code": {
                    "coding": [
                        {
                            "system": "http://snomed.info/sct",
                            "code": "SN52.",
                            "display": "Drug Hypersensitivity NOS"
                        }
                    ]
                },
                "asserter": {
                    "reference": "PractitionerRole/0256ac41-0938-4608-a9ea-1826c6593194"
                },
                "verificationStatus": "confirmed",
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-AllergyIntolerance-1"
                    ],
                    "tag": [
                        {
                            "system": "https://fhir.nhs.uk/Id/ODS-Code",
                            "code": "EMIS99",
                            "display": "GPES Org 20077"
                        }
                    ]
                },
                "id": "78d355a2-cfdd-4d98-a04d-8b33d5a791e2",
                "clinicalStatus": "active",
                "type": "allergy",
                "onsetDateTime": "2013-12-04T00:00:00+00:00",
                "resourceType": "AllergyIntolerance"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73970"
                    }
                ],
                "code": {
                    "coding": [
                        {
                            "system": "http://snomed.info/sct",
                            "code": "TJ53.",
                            "display": "AR - Salicylates"
                        }
                    ]
                },
                "asserter": {
                    "reference": "PractitionerRole/0256ac41-0938-4608-a9ea-1826c6593194"
                },
                "verificationStatus": "confirmed",
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-AllergyIntolerance-1"
                    ],
                    "tag": [
                        {
                            "system": "https://fhir.nhs.uk/Id/ODS-Code",
                            "code": "EMIS99",
                            "display": "GPES Org 20077"
                        }
                    ]
                },
                "id": "95546cd3-d2f6-4313-986d-f538947ffb82",
                "clinicalStatus": "active",
                "type": "allergy",
                "onsetDateTime": "2013-12-04T00:00:00+00:00",
                "resourceType": "AllergyIntolerance"
            }
        },
        {
            "resource": {
                "mode": "snapshot",
                "date": "2026-01-23T11:00:18+00:00",
                "entry": [
                    {
                        "item": {
                            "reference": "AllergyIntolerance/675225a6-aa26-415a-948e-def17bc45e53"
                        }
                    },
                    {
                        "item": {
                            "reference": "AllergyIntolerance/6c0d05c8-f499-479c-a33f-9c4d9a650671"
                        }
                    },
                    {
                        "item": {
                            "reference": "AllergyIntolerance/5e1bfa2e-c2d7-4ff2-b496-5a76121da661"
                        }
                    },
                    {
                        "item": {
                            "reference": "AllergyIntolerance/e57a0b61-741a-4734-b372-dee53b0169e4"
                        }
                    },
                    {
                        "item": {
                            "reference": "AllergyIntolerance/b6e88275-0a9b-4a92-90bb-54e6e5bd5938"
                        }
                    },
                    {
                        "item": {
                            "reference": "AllergyIntolerance/e8565f72-9701-478f-abe3-d88471b8a51a"
                        }
                    },
                    {
                        "item": {
                            "reference": "AllergyIntolerance/c3e0b81c-ecf4-4865-ac02-24f02ab839d2"
                        }
                    },
                    {
                        "item": {
                            "reference": "AllergyIntolerance/78d355a2-cfdd-4d98-a04d-8b33d5a791e2"
                        }
                    },
                    {
                        "item": {
                            "reference": "AllergyIntolerance/95546cd3-d2f6-4313-986d-f538947ffb82"
                        }
                    }
                ],
                "code": {
                    "coding": [
                        {
                            "system": "http://snomed.info/sct",
                            "code": "886921000000105",
                            "display": "Allergies and adverse reaction"
                        }
                    ]
                },
                "orderedBy": {
                    "coding": [
                        {
                            "system": "http://hl7.org/fhir/list-order",
                            "code": " event-date"
                        }
                    ]
                },
                "meta": {
                    "profile": [
                        "https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-List-1"
                    ]
                },
                "subject": {
                    "reference": "Patient/bf3904da-c11f-4004-a774-f6049cb8308e"
                },
                "title": "Active Allergies",
                "resourceType": "List",
                "status": "current"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73914"
                    }
                ],
                "medicationReference": {
                    "reference": "Medication/8560eb71-dc3c-4d8a-b001-33a1e0eb74bb"
                },
                "dosage": [
                    {
                        "text": "One To Be Taken At Night"
                    }
                ],
                "extension": [
                    {
                        "valueDateTime": "2013-11-28T00:00:00+00:00",
                        "url": "https://fhir.hl7.org.uk/STU3/StructureDefinition/Extension-CareConnect-MedicationStatementLastIssueDate-1"
                    }
                ],
                "informationSource": {
                    "reference": "PractitionerRole/c0b86203-a8de-457e-b893-964d4bd558a6"
                },
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-MedicationStatement-1"
                    ],
                    "tag": [
                        {
                            "system": "https://fhir.nhs.uk/Id/ODS-Code",
                            "code": "EMIS99",
                            "display": "GPES Org 20077"
                        }
                    ]
                },
                "subject": {
                    "reference": "Patient/bf3904da-c11f-4004-a774-f6049cb8308e"
                },
                "taken": "unk",
                "id": "887ead30-86a8-47c7-9735-a072b10a9549",
                "dateAsserted": "2013-11-28T00:00:00+00:00",
                "resourceType": "MedicationStatement",
                "status": "active"
            }
        },
        {
            "resource": {
                "code": {
                    "coding": [
                        {
                            "system": "http://snomed.info/sct",
                            "code": "321127001",
                            "display": "nitrazepam 5 milligram/1 each conventional release oral tablet "
                        }
                    ]
                },
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-Medication-1"
                    ]
                },
                "id": "8560eb71-dc3c-4d8a-b001-33a1e0eb74bb",
                "resourceType": "Medication"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73916"
                    }
                ],
                "medicationReference": {
                    "reference": "Medication/698351c4-99b5-441a-9dc4-180f4ffa3541"
                },
                "dosage": [
                    {
                        "text": "One To Be Taken Every 4-6 Hours Up To Four Times A Day"
                    }
                ],
                "extension": [
                    {
                        "valueDateTime": "2014-02-18T00:00:00+00:00",
                        "url": "https://fhir.hl7.org.uk/STU3/StructureDefinition/Extension-CareConnect-MedicationStatementLastIssueDate-1"
                    }
                ],
                "informationSource": {
                    "reference": "PractitionerRole/a23bb040-3d51-47dd-a9cc-a853db62e860"
                },
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-MedicationStatement-1"
                    ],
                    "tag": [
                        {
                            "system": "https://fhir.nhs.uk/Id/ODS-Code",
                            "code": "EMIS99",
                            "display": "GPES Org 20077"
                        }
                    ]
                },
                "subject": {
                    "reference": "Patient/bf3904da-c11f-4004-a774-f6049cb8308e"
                },
                "taken": "unk",
                "id": "b15c4ad2-c28f-4ea9-984a-03b116d09ac7",
                "dateAsserted": "2014-02-18T00:00:00+00:00",
                "resourceType": "MedicationStatement",
                "status": "active"
            }
        },
        {
            "resource": {
                "code": {
                    "coding": [
                        {
                            "system": "http://snomed.info/sct",
                            "code": "322280009",
                            "display": "paracetamol 500 milligram/1 each conventional release oral capsule "
                        }
                    ]
                },
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-Medication-1"
                    ]
                },
                "id": "698351c4-99b5-441a-9dc4-180f4ffa3541",
                "resourceType": "Medication"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73903"
                    }
                ],
                "medicationReference": {
                    "reference": "Medication/797b6c46-6404-48ed-b85f-e144b1913d44"
                },
                "dosage": [
                    {
                        "text": "1 Gram To Be Inserted Each Day"
                    }
                ],
                "extension": [
                    {
                        "valueDateTime": "2014-01-27T00:00:00+00:00",
                        "url": "https://fhir.hl7.org.uk/STU3/StructureDefinition/Extension-CareConnect-MedicationStatementLastIssueDate-1"
                    }
                ],
                "informationSource": {
                    "reference": "PractitionerRole/308d34a6-23b4-48c2-bbf0-38ba493c91e9"
                },
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-MedicationStatement-1"
                    ],
                    "tag": [
                        {
                            "system": "https://fhir.nhs.uk/Id/ODS-Code",
                            "code": "EMIS99",
                            "display": "GPES Org 20077"
                        }
                    ]
                },
                "subject": {
                    "reference": "Patient/bf3904da-c11f-4004-a774-f6049cb8308e"
                },
                "taken": "unk",
                "id": "55d06ed3-6643-44c5-ab15-6015d0683dfd",
                "dateAsserted": "2014-01-27T00:00:00+00:00",
                "resourceType": "MedicationStatement",
                "status": "active"
            }
        },
        {
            "resource": {
                "code": {
                    "coding": [
                        {
                            "system": "http://snomed.info/sct",
                            "code": "329807003",
                            "display": "naproxen 500 milligram/1 each conventional release oral tablet "
                        }
                    ]
                },
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-Medication-1"
                    ]
                },
                "id": "797b6c46-6404-48ed-b85f-e144b1913d44",
                "resourceType": "Medication"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73921"
                    }
                ],
                "medicationReference": {
                    "reference": "Medication/eff439b8-d40e-452e-a1e8-6df9e8b8088a"
                },
                "dosage": [
                    {
                        "text": "1 to be taken 3 times a day"
                    }
                ],
                "extension": [
                    {
                        "valueDateTime": "2014-07-25T00:00:00+00:00",
                        "url": "https://fhir.hl7.org.uk/STU3/StructureDefinition/Extension-CareConnect-MedicationStatementLastIssueDate-1"
                    }
                ],
                "informationSource": {
                    "reference": "PractitionerRole/272d8751-f659-4263-b12e-f6c996a6e19b"
                },
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-MedicationStatement-1"
                    ],
                    "tag": [
                        {
                            "system": "https://fhir.nhs.uk/Id/ODS-Code",
                            "code": "EMIS99",
                            "display": "GPES Org 20077"
                        }
                    ]
                },
                "subject": {
                    "reference": "Patient/bf3904da-c11f-4004-a774-f6049cb8308e"
                },
                "taken": "unk",
                "id": "cc6bc75d-3c1b-48cb-b1be-ffba0d93d3a5",
                "dateAsserted": "2014-07-25T00:00:00+00:00",
                "resourceType": "MedicationStatement",
                "status": "active"
            }
        },
        {
            "resource": {
                "code": {
                    "coding": [
                        {
                            "system": "http://snomed.info/sct",
                            "code": "323734001",
                            "display": "Amoxicillin 125mg/1.25ml oral suspension paediatric"
                        }
                    ]
                },
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-Medication-1"
                    ]
                },
                "id": "eff439b8-d40e-452e-a1e8-6df9e8b8088a",
                "resourceType": "Medication"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73915"
                    }
                ],
                "medicationReference": {
                    "reference": "Medication/66ac8ddd-26a9-4e79-8f7d-ff362110ea45"
                },
                "dosage": [
                    {
                        "text": "Apply To Wet Skin And Rinse"
                    }
                ],
                "extension": [
                    {
                        "valueDateTime": "2013-12-19T00:00:00+00:00",
                        "url": "https://fhir.hl7.org.uk/STU3/StructureDefinition/Extension-CareConnect-MedicationStatementLastIssueDate-1"
                    }
                ],
                "informationSource": {
                    "reference": "PractitionerRole/ce01a137-45e1-4142-84e2-0d16263feacc"
                },
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-MedicationStatement-1"
                    ],
                    "tag": [
                        {
                            "system": "https://fhir.nhs.uk/Id/ODS-Code",
                            "code": "EMIS99",
                            "display": "GPES Org 20077"
                        }
                    ]
                },
                "subject": {
                    "reference": "Patient/bf3904da-c11f-4004-a774-f6049cb8308e"
                },
                "taken": "unk",
                "id": "1d845585-400f-4a10-b8a3-b31dd4dd4621",
                "dateAsserted": "2013-12-02T00:00:00+00:00",
                "resourceType": "MedicationStatement",
                "status": "active"
            }
        },
        {
            "resource": {
                "code": {
                    "coding": [
                        {
                            "system": "http://snomed.info/sct",
                            "code": "3486211000001108",
                            "display": "Liquid paraffin light 70% gel "
                        }
                    ]
                },
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-Medication-1"
                    ]
                },
                "id": "66ac8ddd-26a9-4e79-8f7d-ff362110ea45",
                "resourceType": "Medication"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73922"
                    }
                ],
                "medicationReference": {
                    "reference": "Medication/347b79ea-b691-465d-b548-b6f6fc38c375"
                },
                "dosage": [
                    {
                        "text": "One To Be Taken Every 4-6 Hours Up To Four Times A Day"
                    }
                ],
                "extension": [
                    {
                        "valueDateTime": "2014-07-28T00:00:00+00:00",
                        "url": "https://fhir.hl7.org.uk/STU3/StructureDefinition/Extension-CareConnect-MedicationStatementLastIssueDate-1"
                    }
                ],
                "informationSource": {
                    "reference": "PractitionerRole/272d8751-f659-4263-b12e-f6c996a6e19b"
                },
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-MedicationStatement-1"
                    ],
                    "tag": [
                        {
                            "system": "https://fhir.nhs.uk/Id/ODS-Code",
                            "code": "EMIS99",
                            "display": "GPES Org 20077"
                        }
                    ]
                },
                "subject": {
                    "reference": "Patient/bf3904da-c11f-4004-a774-f6049cb8308e"
                },
                "taken": "unk",
                "id": "8bd34353-6d29-42fa-93e4-d64d34b13f2c",
                "dateAsserted": "2014-07-25T00:00:00+00:00",
                "resourceType": "MedicationStatement",
                "status": "active"
            }
        },
        {
            "resource": {
                "code": {
                    "coding": [
                        {
                            "system": "http://snomed.info/sct",
                            "code": "322280009",
                            "display": "paracetamol 500 milligram/1 each conventional release oral capsule "
                        }
                    ]
                },
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-Medication-1"
                    ]
                },
                "id": "347b79ea-b691-465d-b548-b6f6fc38c375",
                "resourceType": "Medication"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73900"
                    }
                ],
                "medicationReference": {
                    "reference": "Medication/9294f77d-da2e-4051-bf42-b1baaf714139"
                },
                "dosage": [
                    {
                        "text": "One To Be Taken Every 4-6 Hours Up To Four Times A Day"
                    }
                ],
                "extension": [
                    {
                        "valueDateTime": "2014-02-21T00:00:00+00:00",
                        "url": "https://fhir.hl7.org.uk/STU3/StructureDefinition/Extension-CareConnect-MedicationStatementLastIssueDate-1"
                    }
                ],
                "informationSource": {
                    "reference": "PractitionerRole/717b7638-2b37-49eb-83de-4e98a4dfcb73"
                },
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-MedicationStatement-1"
                    ],
                    "tag": [
                        {
                            "system": "https://fhir.nhs.uk/Id/ODS-Code",
                            "code": "EMIS99",
                            "display": "GPES Org 20077"
                        }
                    ]
                },
                "subject": {
                    "reference": "Patient/bf3904da-c11f-4004-a774-f6049cb8308e"
                },
                "taken": "unk",
                "id": "643a0883-efbc-4c49-9695-31a2442e88f3",
                "dateAsserted": "2014-02-21T00:00:00+00:00",
                "resourceType": "MedicationStatement",
                "status": "active"
            }
        },
        {
            "resource": {
                "code": {
                    "coding": [
                        {
                            "system": "http://snomed.info/sct",
                            "code": "322280009",
                            "display": "paracetamol 500 milligram/1 each conventional release oral capsule "
                        }
                    ]
                },
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-Medication-1"
                    ]
                },
                "id": "9294f77d-da2e-4051-bf42-b1baaf714139",
                "resourceType": "Medication"
            }
        },
        {
            "resource": {
                "mode": "snapshot",
                "date": "2026-01-23T11:00:18+00:00",
                "entry": [
                    {
                        "item": {
                            "reference": "MedicationStatement/887ead30-86a8-47c7-9735-a072b10a9549"
                        }
                    },
                    {
                        "item": {
                            "reference": "MedicationStatement/b15c4ad2-c28f-4ea9-984a-03b116d09ac7"
                        }
                    },
                    {
                        "item": {
                            "reference": "MedicationStatement/55d06ed3-6643-44c5-ab15-6015d0683dfd"
                        }
                    },
                    {
                        "item": {
                            "reference": "MedicationStatement/cc6bc75d-3c1b-48cb-b1be-ffba0d93d3a5"
                        }
                    },
                    {
                        "item": {
                            "reference": "MedicationStatement/1d845585-400f-4a10-b8a3-b31dd4dd4621"
                        }
                    },
                    {
                        "item": {
                            "reference": "MedicationStatement/8bd34353-6d29-42fa-93e4-d64d34b13f2c"
                        }
                    },
                    {
                        "item": {
                            "reference": "MedicationStatement/643a0883-efbc-4c49-9695-31a2442e88f3"
                        }
                    }
                ],
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-List-1"
                    ]
                },
                "subject": {
                    "reference": "Patient/bf3904da-c11f-4004-a774-f6049cb8308e"
                },
                "title": "Medication List",
                "resourceType": "List",
                "status": "current"
            }
        },
        {
            "resource": {
                "statusHistory": [
                    {
                        "period": {
                            "start": "2013-05-29T00:00:00+00:00"
                        },
                        "status": "planned"
                    }
                ],
                "period": {
                    "start": "2013-05-29T00:00:00+00:00"
                },
                "managingOrganization": {
                    "reference": "Organization/5ff06392-92cb-4e43-a4cf-d7d683d09197"
                },
                "meta": {
                    "tag": [
                        {
                            "system": "https://fhir.nhs.uk/Id/ODS-Code",
                            "code": "D82027",
                            "display": "HEACHAM GROUP PRACTICE"
                        }
                    ]
                },
                "patient": {
                    "reference": "Patient/bf3904da-c11f-4004-a774-f6049cb8308e"
                },
                "careManager": {
                    "reference": "Practitioner/75bc169c-df60-41d5-9782-5e785529eb40"
                },
                "id": "432cc9ff-57ac-4ee4-a549-806afc1e7e8f",
                "type": [
                    {
                        "coding": [
                            {
                                "system": "http://terminology.hl7.org/CodeSystem/episodeofcare-type",
                                "code": "R",
                                "display": "Regular/GMS"
                            }
                        ]
                    }
                ],
                "resourceType": "EpisodeOfCare",
                "status": "active"
            }
        },
        {
            "resource": {
                "statusHistory": [
                    {
                        "period": {
                            "start": "2013-11-29T00:00:00+00:00"
                        },
                        "status": "planned"
                    }
                ],
                "period": {
                    "start": "2013-11-29T00:00:00+00:00"
                },
                "managingOrganization": {
                    "reference": "Organization/5ff06392-92cb-4e43-a4cf-d7d683d09197"
                },
                "meta": {
                    "tag": [
                        {
                            "system": "https://fhir.nhs.uk/Id/ODS-Code",
                            "code": "EMIS99",
                            "display": "GPES Org 20077"
                        }
                    ]
                },
                "patient": {
                    "reference": "Patient/bf3904da-c11f-4004-a774-f6049cb8308e"
                },
                "careManager": {
                    "reference": "Practitioner/75bc169c-df60-41d5-9782-5e785529eb40"
                },
                "id": "e987c75f-148c-4053-9322-275badea0849",
                "type": [
                    {
                        "coding": [
                            {
                                "system": "http://terminology.hl7.org/CodeSystem/episodeofcare-type",
                                "code": "R",
                                "display": "Regular/GMS"
                            }
                        ]
                    }
                ],
                "resourceType": "EpisodeOfCare",
                "status": "active"
            }
        },
        {
            "resource": {
                "date": "2014-11-04T00:00:00+00:00",
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73958"
                    }
                ],
                "primarySource": true,
                "extension": [
                    {
                        "valueDateTime": "2014-11-04T00:00:00+00:00",
                        "url": "https://fhir.nhs.uk/STU3/StructureDefinition/Extension-CareConnect-GPC-DateRecorded-1"
                    },
                    {
                        "valueCodeableConcept": {
                            "coding": [
                                {
                                    "system": "http://snomed.info/sct",
                                    "code": "65ED.",
                                    "display": "Seasonal influenza vaccination"
                                }
                            ]
                        },
                        "url": null
                    }
                ],
                "practitioner": [
                    {
                        "actor": {
                            "reference": "Practitioner/0bced123-a89d-4ba6-9dc3-03723415aef7"
                        }
                    }
                ],
                "meta": {
                    "profile": [
                        "https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Immunization-1"
                    ],
                    "tag": [
                        {
                            "system": "https://fhir.nhs.uk/Id/ODS-Code",
                            "code": "EMIS99",
                            "display": "GPES Org 20077"
                        }
                    ]
                },
                "patient": {
                    "reference": "Patient/bf3904da-c11f-4004-a774-f6049cb8308e"
                },
                "id": "d6575ce2-03ff-4f2d-b5eb-f097b7d188a1",
                "explanation": {
                    "reason": [
                        {
                            "coding": [
                                {
                                    "system": "http://snomed.info/sct"
                                }
                            ]
                        }
                    ]
                },
                "resourceType": "Immunization",
                "status": "completed"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73562"
                    }
                ],
                "specialty": {
                    "coding": [
                        {
                            "system": "http://orionhealth.com/fhir/apps/specialties",
                            "code": "8HC..",
                            "display": "Refer to hospital casualty"
                        }
                    ]
                },
                "authoredOn": "2014-09-17T00:00:00+00:00",
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-ReferralRequest-1"
                    ],
                    "tag": [
                        {
                            "system": "https://fhir.nhs.uk/Id/ODS-Code",
                            "code": "EMIS99",
                            "display": "GPES Org 20077"
                        }
                    ]
                },
                "subject": {
                    "reference": "Patient/bf3904da-c11f-4004-a774-f6049cb8308e"
                },
                "id": "977d6f4e-37bc-4486-b225-f6681667bfbc",
                "reasonCode": [
                    {
                        "coding": [
                            {
                                "system": "http://snomed.info/sct",
                                "code": "M",
                                "display": "Management advice"
                            }
                        ]
                    }
                ],
                "priority": "routine",
                "intent": "order",
                "resourceType": "ReferralRequest",
                "status": "active"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73563"
                    }
                ],
                "specialty": {
                    "coding": [
                        {
                            "system": "http://orionhealth.com/fhir/apps/specialties",
                            "code": "8H7..",
                            "display": "Other referral"
                        }
                    ]
                },
                "authoredOn": "2013-12-20T00:00:00+00:00",
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-ReferralRequest-1"
                    ],
                    "tag": [
                        {
                            "system": "https://fhir.nhs.uk/Id/ODS-Code",
                            "code": "EMIS99",
                            "display": "GPES Org 20077"
                        }
                    ]
                },
                "subject": {
                    "reference": "Patient/bf3904da-c11f-4004-a774-f6049cb8308e"
                },
                "id": "ffb3310e-8330-447d-ac47-4d861de86fc9",
                "resourceType": "ReferralRequest",
                "status": "active"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73624"
                    }
                ],
                "code": {
                    "coding": [
                        {
                            "extension": [
                                {
                                    "valueString": "O/E - weight",
                                    "url": "descriptionDisplay"
                                }
                            ],
                            "system": "http://snomed.info/sct",
                            "code": "22A..",
                            "display": "O/E - weight"
                        }
                    ]
                },
                "performer": [
                    {
                        "reference": "PractitionerRole/6b80ffb7-0b08-4550-99a8-d3cb1d35e10d"
                    }
                ],
                "effectivePeriod": {
                    "start": "2014-02-03T00:00:00+00:00"
                },
                "meta": {
                    "profile": [
                        "https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Observation-1"
                    ],
                    "tag": [
                        {
                            "system": "https://fhir.nhs.uk/Id/ODS-Code",
                            "code": "EMIS99",
                            "display": "GPES Org 20077"
                        }
                    ]
                },
                "subject": {
                    "reference": "Patient/bf3904da-c11f-4004-a774-f6049cb8308e"
                },
                "id": "fe95ca98-d2f7-4926-9adb-0884bd885f1d",
                "category": [
                    {
                        "text": "vital-signs"
                    }
                ],
                "resourceType": "Observation",
                "status": "final",
                "valueQuantity": {
                    "unit": "kg",
                    "system": "http://unitsofmeasure.org",
                    "value": 101.606
                }
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73663"
                    }
                ],
                "code": {
                    "coding": [
                        {
                            "extension": [
                                {
                                    "valueString": "O/E - Systolic BP reading",
                                    "url": "descriptionDisplay"
                                }
                            ],
                            "system": "http://snomed.info/sct",
                            "code": "2469.",
                            "display": "O/E - Systolic BP reading"
                        }
                    ]
                },
                "performer": [
                    {
                        "reference": "PractitionerRole/a5357357-509f-4b85-94c3-de3edf6f35f3"
                    }
                ],
                "effectivePeriod": {
                    "start": "2013-12-20T00:00:00+00:00"
                },
                "meta": {
                    "profile": [
                        "https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Observation-1"
                    ],
                    "tag": [
                        {
                            "system": "https://fhir.nhs.uk/Id/ODS-Code",
                            "code": "EMIS99",
                            "display": "GPES Org 20077"
                        }
                    ]
                },
                "subject": {
                    "reference": "Patient/bf3904da-c11f-4004-a774-f6049cb8308e"
                },
                "id": "cf42ee05-ae30-4836-9bc7-7395badce9f8",
                "category": [
                    {
                        "text": "vital-signs"
                    }
                ],
                "resourceType": "Observation",
                "status": "final",
                "valueQuantity": {
                    "unit": "mmHg",
                    "system": "http://unitsofmeasure.org",
                    "value": 111.0
                }
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73667"
                    }
                ],
                "code": {
                    "coding": [
                        {
                            "extension": [
                                {
                                    "valueString": "O/E - Diastolic BP reading",
                                    "url": "descriptionDisplay"
                                }
                            ],
                            "system": "http://snomed.info/sct",
                            "code": "246A.",
                            "display": "O/E - Diastolic BP reading"
                        }
                    ]
                },
                "performer": [
                    {
                        "reference": "PractitionerRole/a5357357-509f-4b85-94c3-de3edf6f35f3"
                    }
                ],
                "effectivePeriod": {
                    "start": "2013-12-20T00:00:00+00:00"
                },
                "meta": {
                    "profile": [
                        "https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Observation-1"
                    ],
                    "tag": [
                        {
                            "system": "https://fhir.nhs.uk/Id/ODS-Code",
                            "code": "EMIS99",
                            "display": "GPES Org 20077"
                        }
                    ]
                },
                "subject": {
                    "reference": "Patient/bf3904da-c11f-4004-a774-f6049cb8308e"
                },
                "id": "a1a2515d-0a6a-4635-a59f-2fed6831c030",
                "category": [
                    {
                        "text": "vital-signs"
                    }
                ],
                "resourceType": "Observation",
                "status": "final",
                "valueQuantity": {
                    "unit": "mmHg",
                    "system": "http://unitsofmeasure.org",
                    "value": 75.0
                }
            }
        },
        {
            "resource": {
                "mode": "snapshot",
                "date": "2026-01-23T11:00:18+00:00",
                "entry": [
                    {
                        "item": {
                            "reference": "Observation/fe95ca98-d2f7-4926-9adb-0884bd885f1d"
                        }
                    },
                    {
                        "item": {
                            "reference": "Observation/cf42ee05-ae30-4836-9bc7-7395badce9f8"
                        }
                    },
                    {
                        "item": {
                            "reference": "Observation/a1a2515d-0a6a-4635-a59f-2fed6831c030"
                        }
                    }
                ],
                "orderedBy": {
                    "coding": [
                        {
                            "system": "http://hl7.org/fhir/list-order",
                            "code": "event-date"
                        }
                    ]
                },
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-List-1"
                    ]
                },
                "subject": {
                    "reference": "Patient/bf3904da-c11f-4004-a774-f6049cb8308e"
                },
                "title": "Miscellaneous record",
                "resourceType": "List",
                "status": "current"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.nhs.uk/Id/ods-organization-code",
                        "value": "D82027"
                    },
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "1"
                    }
                ],
                "address": [
                    {
                        "postalCode": "PE31 7EX"
                    }
                ],
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-Organization-1"
                    ]
                },
                "name": "HEACHAM GROUP PRACTICE",
                "id": "5ff06392-92cb-4e43-a4cf-d7d683d09197",
                "resourceType": "Organization"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.nhs.uk/Id/ods-organization-code",
                        "value": "EMIS99"
                    },
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "14089"
                    }
                ],
                "address": [
                    {
                        "postalCode": "LS299EN"
                    }
                ],
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-Organization-1"
                    ]
                },
                "name": "GPES Org 20077",
                "id": "e8cbeaa1-9ef0-4c0e-9d36-7c942c78ca8d",
                "resourceType": "Organization"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.nhs.uk/Id/sds-user-id"
                    },
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73891"
                    }
                ],
                "meta": {
                    "lastUpdated": "2026-01-23T11:00:18.501+00:00",
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-Practitioner-1"
                    ]
                },
                "name": [
                    {
                        "given": [
                            "Lina"
                        ],
                        "use": "usual",
                        "prefix": [
                            "Dr"
                        ],
                        "family": "LAWRENS"
                    }
                ],
                "id": "f21dc34a-5d6d-4b86-b1ff-83521094bea7",
                "resourceType": "Practitioner"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73891"
                    }
                ],
                "code": [
                    {
                        "coding": [
                            {
                                "system": "https://fhir.hl7.org.uk/STU3/CodeSystem/CareConnect-SDSJobRoleName-1",
                                "code": "R0260",
                                "display": "General Medical Practitioner"
                            }
                        ]
                    }
                ],
                "practitioner": {
                    "reference": "Practitioner/f21dc34a-5d6d-4b86-b1ff-83521094bea7"
                },
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-PractitionerRole-1"
                    ]
                },
                "organization": {
                    "reference": "Organization/e8cbeaa1-9ef0-4c0e-9d36-7c942c78ca8d"
                },
                "id": "a5357357-509f-4b85-94c3-de3edf6f35f3",
                "resourceType": "PractitionerRole"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.nhs.uk/Id/sds-user-id"
                    },
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73959"
                    }
                ],
                "meta": {
                    "lastUpdated": "2026-01-23T11:00:18.484+00:00",
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-Practitioner-1"
                    ]
                },
                "name": [
                    {
                        "given": [
                            "david"
                        ],
                        "use": "usual",
                        "prefix": [
                            "Dr"
                        ],
                        "family": "BINNEY"
                    }
                ],
                "id": "feaa62a1-f743-4b45-b686-6d29e1953dcc",
                "resourceType": "Practitioner"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73959"
                    }
                ],
                "code": [
                    {
                        "coding": [
                            {
                                "system": "https://fhir.hl7.org.uk/STU3/CodeSystem/CareConnect-SDSJobRoleName-1",
                                "code": "R0260",
                                "display": "General Medical Practitioner"
                            }
                        ]
                    }
                ],
                "practitioner": {
                    "reference": "Practitioner/feaa62a1-f743-4b45-b686-6d29e1953dcc"
                },
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-PractitionerRole-1"
                    ]
                },
                "organization": {
                    "reference": "Organization/e8cbeaa1-9ef0-4c0e-9d36-7c942c78ca8d"
                },
                "id": "717b7638-2b37-49eb-83de-4e98a4dfcb73",
                "resourceType": "PractitionerRole"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.nhs.uk/Id/sds-user-id"
                    },
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "871"
                    }
                ],
                "meta": {
                    "lastUpdated": "2026-01-23T11:00:18.457+00:00",
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-Practitioner-1"
                    ]
                },
                "name": [
                    {
                        "given": [
                            "david"
                        ],
                        "use": "usual",
                        "prefix": [
                            "Mr"
                        ],
                        "family": "BINNEY"
                    }
                ],
                "id": "75bc169c-df60-41d5-9782-5e785529eb40",
                "resourceType": "Practitioner"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "871"
                    }
                ],
                "code": [
                    {
                        "coding": [
                            {
                                "system": "https://fhir.hl7.org.uk/STU3/CodeSystem/CareConnect-SDSJobRoleName-1",
                                "code": "R0260",
                                "display": "General Medical Practitioner"
                            }
                        ]
                    }
                ],
                "practitioner": {
                    "reference": "Practitioner/75bc169c-df60-41d5-9782-5e785529eb40"
                },
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-PractitionerRole-1"
                    ]
                },
                "id": "9dd7249c-b05a-47ec-a24e-a8ef0d88d39f",
                "resourceType": "PractitionerRole"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.nhs.uk/Id/sds-user-id"
                    },
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "15306"
                    }
                ],
                "meta": {
                    "lastUpdated": "2026-01-23T11:00:18.483+00:00",
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-Practitioner-1"
                    ]
                },
                "name": [
                    {
                        "given": [
                            "david"
                        ],
                        "use": "usual",
                        "prefix": [
                            "Mr"
                        ],
                        "family": "BINNEY"
                    }
                ],
                "id": "6ab29e2c-c1b9-4fd6-b878-9a8f5435da0f",
                "resourceType": "Practitioner"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "15306"
                    }
                ],
                "code": [
                    {
                        "coding": [
                            {
                                "system": "https://fhir.hl7.org.uk/STU3/CodeSystem/CareConnect-SDSJobRoleName-1",
                                "code": "R0260",
                                "display": "General Medical Practitioner"
                            }
                        ]
                    }
                ],
                "practitioner": {
                    "reference": "Practitioner/6ab29e2c-c1b9-4fd6-b878-9a8f5435da0f"
                },
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-PractitionerRole-1"
                    ]
                },
                "organization": {
                    "reference": "Organization/e8cbeaa1-9ef0-4c0e-9d36-7c942c78ca8d"
                },
                "id": "272d8751-f659-4263-b12e-f6c996a6e19b",
                "resourceType": "PractitionerRole"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.nhs.uk/Id/sds-user-id"
                    },
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "39822"
                    }
                ],
                "meta": {
                    "lastUpdated": "2026-01-23T11:00:18.492+00:00",
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-Practitioner-1"
                    ]
                },
                "name": [
                    {
                        "use": "usual",
                        "family": "System User"
                    }
                ],
                "id": "0bced123-a89d-4ba6-9dc3-03723415aef7",
                "resourceType": "Practitioner"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "39822"
                    }
                ],
                "code": [
                    {
                        "coding": [
                            {
                                "system": "https://fhir.hl7.org.uk/STU3/CodeSystem/CareConnect-SDSJobRoleName-1",
                                "code": "R5007",
                                "display": "System Administrator"
                            }
                        ]
                    }
                ],
                "practitioner": {
                    "reference": "Practitioner/0bced123-a89d-4ba6-9dc3-03723415aef7"
                },
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-PractitionerRole-1"
                    ]
                },
                "id": "cf820291-a019-41a1-b6c6-8e547b45ef31",
                "resourceType": "PractitionerRole"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.nhs.uk/Id/sds-user-id"
                    },
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "15951"
                    }
                ],
                "meta": {
                    "lastUpdated": "2026-01-23T11:00:18.476+00:00",
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-Practitioner-1"
                    ]
                },
                "name": [
                    {
                        "given": [
                            "Andy"
                        ],
                        "use": "usual",
                        "prefix": [
                            "Mr"
                        ],
                        "family": "MARSHALL SEAS"
                    }
                ],
                "id": "eb7064c6-c774-41fe-9d37-ef6c15bb2f9e",
                "resourceType": "Practitioner"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "15951"
                    }
                ],
                "code": [
                    {
                        "coding": [
                            {
                                "system": "https://fhir.hl7.org.uk/STU3/CodeSystem/CareConnect-SDSJobRoleName-1",
                                "code": "R0260",
                                "display": "General Medical Practitioner"
                            }
                        ]
                    }
                ],
                "practitioner": {
                    "reference": "Practitioner/eb7064c6-c774-41fe-9d37-ef6c15bb2f9e"
                },
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-PractitionerRole-1"
                    ]
                },
                "organization": {
                    "reference": "Organization/e8cbeaa1-9ef0-4c0e-9d36-7c942c78ca8d"
                },
                "id": "0256ac41-0938-4608-a9ea-1826c6593194",
                "resourceType": "PractitionerRole"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.nhs.uk/Id/sds-user-id"
                    },
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "65202"
                    }
                ],
                "meta": {
                    "lastUpdated": "2026-01-23T11:00:18.476+00:00",
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-Practitioner-1"
                    ]
                },
                "name": [
                    {
                        "given": [
                            "Richard"
                        ],
                        "use": "usual",
                        "prefix": [
                            "Mr"
                        ],
                        "family": "HAWLEY"
                    }
                ],
                "id": "1fe78207-9dbd-4d8c-9336-9f717fa4445c",
                "resourceType": "Practitioner"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "65202"
                    }
                ],
                "code": [
                    {
                        "coding": [
                            {
                                "system": "https://fhir.hl7.org.uk/STU3/CodeSystem/CareConnect-SDSJobRoleName-1",
                                "code": "R0260",
                                "display": "General Medical Practitioner"
                            }
                        ]
                    }
                ],
                "practitioner": {
                    "reference": "Practitioner/1fe78207-9dbd-4d8c-9336-9f717fa4445c"
                },
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-PractitionerRole-1"
                    ]
                },
                "organization": {
                    "reference": "Organization/e8cbeaa1-9ef0-4c0e-9d36-7c942c78ca8d"
                },
                "id": "66d19ac6-ce1b-4623-b860-372634dca807",
                "resourceType": "PractitionerRole"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.nhs.uk/Id/sds-user-id"
                    },
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73972"
                    }
                ],
                "meta": {
                    "lastUpdated": "2026-01-23T11:00:18.483+00:00",
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-Practitioner-1"
                    ]
                },
                "name": [
                    {
                        "given": [
                            "Martin"
                        ],
                        "use": "usual",
                        "prefix": [
                            "Mr"
                        ],
                        "family": "CAIN"
                    }
                ],
                "id": "c806a152-60b4-47fb-8aaf-d54aacef3596",
                "resourceType": "Practitioner"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73972"
                    }
                ],
                "code": [
                    {
                        "coding": [
                            {
                                "system": "https://fhir.hl7.org.uk/STU3/CodeSystem/CareConnect-SDSJobRoleName-1",
                                "code": "R0260",
                                "display": "General Medical Practitioner"
                            }
                        ]
                    }
                ],
                "practitioner": {
                    "reference": "Practitioner/c806a152-60b4-47fb-8aaf-d54aacef3596"
                },
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-PractitionerRole-1"
                    ]
                },
                "organization": {
                    "reference": "Organization/e8cbeaa1-9ef0-4c0e-9d36-7c942c78ca8d"
                },
                "id": "308d34a6-23b4-48c2-bbf0-38ba493c91e9",
                "resourceType": "PractitionerRole"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.nhs.uk/Id/sds-user-id"
                    },
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73974"
                    }
                ],
                "meta": {
                    "lastUpdated": "2026-01-23T11:00:18.498+00:00",
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-Practitioner-1"
                    ]
                },
                "name": [
                    {
                        "given": [
                            "Craig"
                        ],
                        "use": "usual",
                        "prefix": [
                            "Mr"
                        ],
                        "family": "TURNER"
                    }
                ],
                "id": "374e8979-2c5a-4c11-a871-9386b07c9373",
                "resourceType": "Practitioner"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73974"
                    }
                ],
                "code": [
                    {
                        "coding": [
                            {
                                "system": "https://fhir.hl7.org.uk/STU3/CodeSystem/CareConnect-SDSJobRoleName-1",
                                "code": "R0260",
                                "display": "General Medical Practitioner"
                            }
                        ]
                    }
                ],
                "practitioner": {
                    "reference": "Practitioner/374e8979-2c5a-4c11-a871-9386b07c9373"
                },
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-PractitionerRole-1"
                    ]
                },
                "organization": {
                    "reference": "Organization/e8cbeaa1-9ef0-4c0e-9d36-7c942c78ca8d"
                },
                "id": "6b80ffb7-0b08-4550-99a8-d3cb1d35e10d",
                "resourceType": "PractitionerRole"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.nhs.uk/Id/sds-user-id"
                    },
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73881"
                    }
                ],
                "meta": {
                    "lastUpdated": "2026-01-23T11:00:18.481+00:00",
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-Practitioner-1"
                    ]
                },
                "name": [
                    {
                        "given": [
                            "ryan"
                        ],
                        "use": "usual",
                        "prefix": [
                            "Mr"
                        ],
                        "family": "WALL"
                    }
                ],
                "id": "cd5e6337-65a7-436a-aeb0-f4431b3f4086",
                "resourceType": "Practitioner"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73881"
                    }
                ],
                "code": [
                    {
                        "coding": [
                            {
                                "system": "https://fhir.hl7.org.uk/STU3/CodeSystem/CareConnect-SDSJobRoleName-1",
                                "code": "R0260",
                                "display": "General Medical Practitioner"
                            }
                        ]
                    }
                ],
                "practitioner": {
                    "reference": "Practitioner/cd5e6337-65a7-436a-aeb0-f4431b3f4086"
                },
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-PractitionerRole-1"
                    ]
                },
                "organization": {
                    "reference": "Organization/e8cbeaa1-9ef0-4c0e-9d36-7c942c78ca8d"
                },
                "id": "c0b86203-a8de-457e-b893-964d4bd558a6",
                "resourceType": "PractitionerRole"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.nhs.uk/Id/sds-user-id"
                    },
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73880"
                    }
                ],
                "meta": {
                    "lastUpdated": "2026-01-23T11:00:18.475+00:00",
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-Practitioner-1"
                    ]
                },
                "name": [
                    {
                        "given": [
                            "Christopher"
                        ],
                        "use": "usual",
                        "prefix": [
                            "Mr"
                        ],
                        "family": "ROBERTS"
                    }
                ],
                "id": "e2e37d82-b709-4df2-b8c1-a63dcfe1837d",
                "resourceType": "Practitioner"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73880"
                    }
                ],
                "code": [
                    {
                        "coding": [
                            {
                                "system": "https://fhir.hl7.org.uk/STU3/CodeSystem/CareConnect-SDSJobRoleName-1",
                                "code": "R0260",
                                "display": "General Medical Practitioner"
                            }
                        ]
                    }
                ],
                "practitioner": {
                    "reference": "Practitioner/e2e37d82-b709-4df2-b8c1-a63dcfe1837d"
                },
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-PractitionerRole-1"
                    ]
                },
                "organization": {
                    "reference": "Organization/e8cbeaa1-9ef0-4c0e-9d36-7c942c78ca8d"
                },
                "id": "89adffa3-0342-407e-9f65-4f2ff39cfebf",
                "resourceType": "PractitionerRole"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.nhs.uk/Id/sds-user-id"
                    },
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73976"
                    }
                ],
                "meta": {
                    "lastUpdated": "2026-01-23T11:00:18.484+00:00",
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-Practitioner-1"
                    ]
                },
                "name": [
                    {
                        "given": [
                            "Peter"
                        ],
                        "use": "usual",
                        "prefix": [
                            "Dr"
                        ],
                        "family": "ROHAT"
                    }
                ],
                "id": "f5822658-f24c-4620-94b8-efd4ba66c850",
                "resourceType": "Practitioner"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73976"
                    }
                ],
                "code": [
                    {
                        "coding": [
                            {
                                "system": "https://fhir.hl7.org.uk/STU3/CodeSystem/CareConnect-SDSJobRoleName-1",
                                "code": "R0260",
                                "display": "General Medical Practitioner"
                            }
                        ]
                    }
                ],
                "practitioner": {
                    "reference": "Practitioner/f5822658-f24c-4620-94b8-efd4ba66c850"
                },
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-PractitionerRole-1"
                    ]
                },
                "organization": {
                    "reference": "Organization/e8cbeaa1-9ef0-4c0e-9d36-7c942c78ca8d"
                },
                "id": "ce01a137-45e1-4142-84e2-0d16263feacc",
                "resourceType": "PractitionerRole"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.nhs.uk/Id/sds-user-id"
                    },
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73917"
                    }
                ],
                "meta": {
                    "lastUpdated": "2026-01-23T11:00:18.482+00:00",
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-Practitioner-1"
                    ]
                },
                "name": [
                    {
                        "given": [
                            "David"
                        ],
                        "use": "usual",
                        "prefix": [
                            "Mr"
                        ],
                        "family": "BINNEY"
                    }
                ],
                "id": "c45db0a4-b488-4435-aa06-f44dead35a63",
                "resourceType": "Practitioner"
            }
        },
        {
            "resource": {
                "identifier": [
                    {
                        "system": "https://fhir.hl7.org.uk/Id/dds",
                        "value": "73917"
                    }
                ],
                "code": [
                    {
                        "coding": [
                            {
                                "system": "https://fhir.hl7.org.uk/STU3/CodeSystem/CareConnect-SDSJobRoleName-1",
                                "code": "R0260",
                                "display": "General Medical Practitioner"
                            }
                        ]
                    }
                ],
                "practitioner": {
                    "reference": "Practitioner/c45db0a4-b488-4435-aa06-f44dead35a63"
                },
                "meta": {
                    "profile": [
                        "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-PractitionerRole-1"
                    ]
                },
                "organization": {
                    "reference": "Organization/e8cbeaa1-9ef0-4c0e-9d36-7c942c78ca8d"
                },
                "id": "a23bb040-3d51-47dd-a9cc-a853db62e860",
                "resourceType": "PractitionerRole"
            }
        }
    ],
        "meta": {
        "profile": [
            "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-StructuredRecord-Bundle-1"
        ]
    },
    "type": "collection",
        "resourceType": "Bundle"
}