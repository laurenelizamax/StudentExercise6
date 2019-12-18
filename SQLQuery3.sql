--SELECT c.Id AS CohortId, c.CohortName, s.Id AS StudentId, 
--                                        s.FirstName AS SFirstName, s.LastName AS SLastName,
--                                        s.SlackHandle AS SSlackHandle, s.CohortId, e.Id AS ExerciseId, 
--                                        e.ExerciseName, e.ExerciseLanguage,
--                                        i.Id AS InstructorId, i.FirstName AS IFirstName, 
--                                        i.LastName AS ILastName, i.SlackHandle AS ISlackHandle, 
--                                        i.CohortId FROM Cohort c
--                                        INNER JOIN Student s ON c.Id = s.CohortId
--                                        LEFT JOIN StudentExercise se ON s.Id = se.StudentId
--                                        LEFT JOIN Exercise e ON e.Id = se.ExerciseId
--                                        LEFT JOIN Instructor i ON c.Id = i.CohortId
-- Like with first and last name
--Where LastName LIKE 'Maxwel%';
--Where LastName LIKE 'Maxwell';

--SELECT * FROM Student
--WHERE LastName LIKE '%axwel%';

-- Order by ascending
--SELECT Id, FirstName, LastName, SlackHandle, CohortId
--FROM Student
--ORDER BY LastName;


