using Szlakomat.TripRecommendation.Application.Planning;

// ── Przykładowy endpoint — tworzenie snapshotu planowania ─────────────────────
//
// Dodaj do Program.cs obok istniejącego /api/normalization/attractions.
// Endpoint demonstruje przepływ:
//   HTTP POST → PlanningInputRequest → CorrectedPlanningInputFactory → snapshot → odpowiedź
