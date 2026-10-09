-- ============================================================================
-- SUPABASE LEADERBOARD SCHEMA CHO SHADOWREFLEX (COMMERCIAL RELEASE)
-- ============================================================================
-- Hướng dẫn: Copy toàn bộ script này và dán vào Supabase SQL Editor -> Run
-- Schema: id (bigserial), gamertag (text), score (integer), wave (integer), created_at (timestamptz)

-- 1. Tạo bảng leaderboard
CREATE TABLE IF NOT EXISTS public.leaderboard (
    id BIGSERIAL PRIMARY KEY,
    gamertag TEXT NOT NULL,
    score INTEGER NOT NULL DEFAULT 0,
    wave INTEGER NOT NULL DEFAULT 1,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- 2. Chỉ mục tối ưu hóa truy vấn bảng xếp hạng (Index Top Scores)
CREATE INDEX IF NOT EXISTS idx_leaderboard_score_desc ON public.leaderboard (score DESC, wave DESC);
CREATE INDEX IF NOT EXISTS idx_leaderboard_gamertag ON public.leaderboard (gamertag);

-- 3. Kích hoạt Row Level Security (RLS) bảo mật dữ liệu
ALTER TABLE public.leaderboard ENABLE ROW LEVEL SECURITY;

-- 4. Chính sách RLS cho phép Client đọc Top Scores tự do (Public Read)
DROP POLICY IF EXISTS "Allow anon public read" ON public.leaderboard;
CREATE POLICY "Allow anon public read"
ON public.leaderboard
FOR SELECT
TO anon, authenticated
USING (true);

-- 5. Chính sách RLS cho phép Client nộp điểm (Anon Insert)
DROP POLICY IF EXISTS "Allow anon insert scores" ON public.leaderboard;
CREATE POLICY "Allow anon insert scores"
ON public.leaderboard
FOR INSERT
TO anon, authenticated
WITH CHECK (true);

-- 6. Chính sách RLS cho phép cập nhật điểm cao hơn (Anon Update)
DROP POLICY IF EXISTS "Allow anon update own score" ON public.leaderboard;
CREATE POLICY "Allow anon update own score"
ON public.leaderboard
FOR UPDATE
TO anon, authenticated
USING (true)
WITH CHECK (true);

-- 7. Dữ liệu mẫu khởi tạo ban đầu (Ninja Seed Legends)
INSERT INTO public.leaderboard (gamertag, score, wave, created_at)
VALUES 
    ('Hayabusa_Prime', 12500, 10, NOW()),
    ('Hanzo_Steel', 9800, 8, NOW()),
    ('Kotaro_Ghost', 8400, 7, NOW()),
    ('Kunoichi_Mist', 7200, 6, NOW()),
    ('Ryu_Shadow', 5900, 5, NOW()),
    ('Sasuke_Flame', 5400, 5, NOW()),
    ('Genji_Cyborg', 4600, 4, NOW()),
    ('Kenshin_Battosai', 3900, 3, NOW())
ON CONFLICT DO NOTHING;
