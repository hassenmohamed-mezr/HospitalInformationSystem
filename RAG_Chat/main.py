from fastapi import FastAPI
from openai import chat
from pydantic import BaseModel
import os
import numpy as np
import faiss
from sentence_transformers import SentenceTransformer
import google.generativeai as genai

app = FastAPI()


# =========================
# =========================
# RAG + GEMINI (IMPROVED ARCHITECTURE)
# =========================



# =========================
# 1. GEMINI CONFIG (SECURE)
# =========================
GEMINI_API_KEY = "AIzaSyBIW0uuZ28Q5c4-IsBTPP5VtR9Tyw7sPd4"

if not GEMINI_API_KEY:
    raise ValueError("Missing GEMINI_API_KEY in environment variables")

genai.configure(api_key=GEMINI_API_KEY)

llm = genai.GenerativeModel(
    model_name="gemini-2.5-flash"
)

# =========================
# 2. KNOWLEDGE BASE (SYSTEM-LEVEL STRUCTURE)
# =========================

docs = [

# =========================
# SYSTEM OVERVIEW (USER VIEW)
# =========================
"""
HOSPITAL SYSTEM OVERVIEW:

This system helps manage hospital operations in a simple and organized way.

It connects:
- Patients
- Doctors
- Reception staff
- Administrators

MAIN PURPOSE:
To schedule appointments, manage patient visits, and keep medical records organized.

HOW THE SYSTEM WORKS:
You log in → You go to your dashboard → You perform tasks based on your role.
""",

# =========================
# ROLES EXPLAINED SIMPLY
# =========================
"""
USER ROLES:

ADMIN:
- Manages system users (doctors, reception, etc.)
- Controls system settings
- Oversees overall hospital operations

RECEPTION:
- Registers new patients
- Schedules appointments between doctors and patients
- Manages daily booking and organization

DOCTOR:
- Views daily patient appointments
- Confirms and handles patient consultations
- Records medical visits (diagnosis, notes, treatment)
- Views patient medical history
""",

# =========================
# DOCTOR EXPERIENCE FLOW
# =========================
"""
DOCTOR EXPERIENCE FLOW:

As a doctor, your daily work is:

1. Open your dashboard
2. View today's patient appointments
3. Select a patient appointment
4. Confirm the appointment when the patient arrives
5. Start the medical consultation
6. Record diagnosis and treatment
7. Save the visit (medical record)
8. View patient history if needed

SUMMARY:
Your work starts with appointments and ends with medical records (visits).
""",

# =========================
# APPOINTMENTS (USER VIEW)
# =========================
"""
APPOINTMENTS:

An appointment is a scheduled meeting between a doctor and a patient.

WHAT YOU SEE:
- Patient name
- Doctor name
- Date and time
- Status (Pending, Confirmed, Completed, Cancelled)

WHO USES IT:
- Reception creates appointments
- Doctors confirm and complete them

RULES:
- Appointments are scheduled in advance
- Each appointment belongs to one doctor and one patient
""",

# =========================
# VISITS (MEDICAL RECORDS)
# =========================
"""
VISITS:

A visit is the final result of a completed appointment.

WHAT IT CONTAINS:
- Diagnosis (what the doctor found)
- Notes (doctor observations)
- Treatment (what was prescribed or recommended)

IMPORTANT:
- Visits are created only after a patient is seen
- They become part of the patient's permanent medical history
""",

# =========================
# PATIENT SYSTEM
# =========================
"""
PATIENTS:

Patients are the core of the system.

WHAT CAN BE DONE:
- Register new patients
- View patient details
- View full medical history (past visits)
- Link patients to appointments and doctors

SIMPLY:
Every medical action in the system is connected to a patient.
""",

# =========================
# SIMPLE SYSTEM STORY
# =========================
"""
HOW THE SYSTEM WORKS (SIMPLE STORY):

A patient comes to the hospital →
Reception books an appointment →
Doctor sees the appointment →
Doctor examines the patient →
Doctor writes medical notes →
A visit is saved →
Patient history is updated permanently
"""
]

# =========================
# 3. EMBEDDING MODEL
# =========================
embedder = SentenceTransformer("all-MiniLM-L6-v2")

doc_embeddings = embedder.encode(
    docs,
    normalize_embeddings=True
)

# =========================
# 4. FAISS INDEX (COSINE SIMILARITY)
# =========================
dim = doc_embeddings.shape[1]
index = faiss.IndexFlatIP(dim)  # ⬅️ أفضل من L2 هنا
index.add(np.array(doc_embeddings).astype("float32"))

# =========================
# 5. RETRIEVAL ENGINE
# =========================
def retrieve(query, k=3):
    q_emb = embedder.encode([query], normalize_embeddings=True)
    scores, idx = index.search(np.array(q_emb).astype("float32"), k)

    results = []
    for i in idx[0]:
        if i != -1:
            results.append(docs[i])

    return results

# =========================
# 6. QUERY ENRICHMENT (IMPORTANT)
# =========================
def enrich_query(query):
        return f"""
    SYSTEM DESIGN QUESTION:
    Explain as a real software system with:
    - Pages
    - Navigation flow
    - Role behavior
    - Step-by-step user journey

    USER QUERY:
    {query}
    """

# =========================
# 7. PROMPT ENGINE (STRICT)
# =========================
def build_prompt(query, context_docs):
    context = "\n\n".join(context_docs)

    return f"""
You are a helpful hospital system assistant.

Your goal is to explain the system clearly like a real helpful human would explain it to a new doctor.

--------------------------------------------------
RULES:
--------------------------------------------------
- Use ONLY the provided context (no external assumptions)
- Do NOT hallucinate missing features
- Keep answers natural and easy to understand
- You may use simple friendly language
- You can explain using short paragraphs
- Avoid rigid templates or forced numbering unless useful
- No technical or robotic tone
- No unnecessary repetition

--------------------------------------------------
STYLE:
--------------------------------------------------
- Natural human explanation
- Clear and simple
- Like a senior doctor explaining to a new doctor
- Friendly but professional

--------------------------------------------------
CONTEXT:
--------------------------------------------------
{context}

--------------------------------------------------
QUESTION:
--------------------------------------------------
{query}

--------------------------------------------------
ANSWER:
--------------------------------------------------
"""

# =========================
# 8. LLM CALL (SAFE)
# =========================
def call_llm(prompt):
    try:
        response = llm.generate_content(
            prompt,
            generation_config={
                "temperature": 0.2,
                "top_p": 0.9
            }
        )

        return response.text

    except Exception as e:
        return f"LLM_ERROR: {str(e)}"

# =========================
# 9. PIPELINE
# =========================
def chat(query):
    query = enrich_query(query)

    context = retrieve(query, k=3)

    prompt = build_prompt(query, context)

    answer = call_llm(prompt)

    return answer, context

# =========================
# 10. RUN LOOP
# =========================
if __name__ == "__main__":
    print("RAG SYSTEM READY (IMPROVED VERSION)")

    while True:
        q = input("\nAsk: ")

        answer, context = chat(q)

        print("\nANSWER:\n")
        print(answer)

        print("\nCONTEXT USED:\n")
        for c in context:
            print("-", c)



class QueryRequest(BaseModel):
    question: str

@app.post("/ask")
def ask(req: QueryRequest):

    answer, context = chat(req.question)

    return {
        "answer": answer,
        "context": context
    }