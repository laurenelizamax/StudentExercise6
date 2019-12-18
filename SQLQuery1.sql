SELECT s.Id, s.FirstName, s.LastName, s.SlackHandle, s.CohortId,
i.Id, i.FirstName, i.LastName, i.SlackHandle, i.CohortId, c.Id, c.CohortName
FROM Cohort c 
LEFT JOIN Student s ON c.Id = s.CohortId
LEFT JOIN Instructor i ON c.Id = i.CohortId;